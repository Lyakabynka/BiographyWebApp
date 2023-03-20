using BiographyWebApp.Abstractions;
using BiographyWebApp.Database.DbContexts;
using BiographyWebApp.Database.Repositories;
using BiographyWebApp.Models;
using BiographyWebApp.Services;
using BiographyWebApp.Services.Hashing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace BiographyWebApp.Controllers
{
    public class UserController : Controller
    {
        private IAppRepository _repo;
        private EmailSenderService _emailSenderService;
        private AuthService _authService;
        public UserController(IAppRepository repo, EmailSenderService emailSenderService, AuthService authService) 
        {
            _repo = repo;
            _emailSenderService = emailSenderService;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterModel userRegisterModel)
        {
            bool Status = false;
            string Message = string.Empty;
            //Model Validation
           
            if (ModelState.IsValid)
            {
                #region //If Email exists throw Error
                if(await _repo.EmailExistsAsync(userRegisterModel.Email))
                {
                    ModelState.AddModelError("EmailExists", "Email already exists");
                    return View(userRegisterModel);
                }
                #endregion

                #region //If not, create User object ( Password Hashing + Generating Activation Code )
                User user = new User() {
                    FirstName = userRegisterModel.FirstName,
                    LastName = userRegisterModel.LastName,
                    Email = userRegisterModel.Email,
                    DateOfBirth = userRegisterModel.DateOfBirth,
                    Role = UserRole.User,
                    Password = Encryptor.Hash(userRegisterModel.Password),
                    IsEmailVerified = false,
                    ActivationCode = new ActivationCode(),
                    ResetPasswordCode = null,
                };
                #endregion

                #region //Save User to Database + DbContext.SaveChangesAsync()
                _repo.AddUserAsync(user);
                #endregion

                #region //Send Email to User
                _emailSenderService.SendVerificationLinkEmailAsync(
                    HttpContext,
                    user.Email, 
                    user.ActivationCode.Code.ToString());
                #endregion

                Message = "Registration has been successfully completed. Account activation link has been send to your email.";
                Status = true;
            }
            else
            {
                Message = "Invalid Credentials";
            }

            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View();
        }

        [HttpGet]
        [Route("User/VerifyAccount/{ActivationCode}")]
        public async Task<IActionResult> VerifyAccount(string ActivationCode)
        {
            if (!Guid.TryParse(ActivationCode, out Guid activationCode))
            {
                return NotFound();
            }

            User? user = await _repo.GetUserByActivationCodeAsync(activationCode);
            if (user is not null)
            {
                await _repo.DeleteActivationCodeAndVerifyUserAsync(user); // makes ( VerifiedAccount = True ) and saves the database.

                return View();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = "/")
        {
            UserLoginModel userLoginModel = new UserLoginModel();
            userLoginModel.ReturnUrl = returnUrl;
            return View(userLoginModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel userLoginModel)
        {
            string Message = string.Empty;

            User? user = await _repo.GetUserByEmailAsync(userLoginModel.Email);
            if (user is not null && user.Password == Encryptor.Hash(userLoginModel.Password))
            {
                if(user.IsEmailVerified) 
                {
                    #region //Adding a cookie
                    ClaimsPrincipal principal = _authService.GenerateClaimsPrincipal(user, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                        new AuthenticationProperties()
                        {
                            IsPersistent = userLoginModel.RememberAuthorization // if browser is closed cookie remains. 
                        });
                    #endregion
                    
                    if (Url.IsLocalUrl(userLoginModel.ReturnUrl))
                    {
                        return LocalRedirect(userLoginModel.ReturnUrl);
                    }
                    else
                    {
                        return LocalRedirect("/");
                    }
                }
                else
                {
                    Message = "Verify  your account via link, sent to your email.";
                }
            }
            else
            {
                Message = "Invalid Email or/and Passoword.";
            }

            ViewBag.Message = Message;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            string Message = string.Empty;
            bool Status = false;
            User? user = await _repo.GetUserByEmailAsync(Email);
            if (user is not null) 
            {
                #region // Creating new instance of ResetPasswordCode and assigning it to user + DbContext.SaveChangesAsync()
                await _repo.AddResetPasswordCodeForUserAsync(user);
                #endregion

                #region // Sending ForgotPassword Link to user's email
                _emailSenderService.SendForgotPasswordLinkEmailAsync(
                    HttpContext,
                    Email, 
                    user.ResetPasswordCode.Code.ToString());
                #endregion

                Message = "Reset password link has been sent to your email.";
                Status = true;
            }
            else
            {
                Message = "Email does not exist.";
            }
            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View();
        }

        [HttpGet]
        [Route("User/ResetPassword/{ResetPasswordCode}")]
        public async Task<IActionResult> ResetPassword(string ResetPasswordCode)
        {
            if (!Guid.TryParse(ResetPasswordCode, out Guid activationCode))
            {
                return NotFound();
            }

            User? user = await _repo.GetUserByResetPasswordCodeAsync(activationCode);
            if (user is not null)
            {
                ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
                resetPasswordModel.ResetPasswordCode = ResetPasswordCode;
                return View(resetPasswordModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            string Message = string.Empty;
            bool Status = false;
            if (ModelState.IsValid)
            {
                User? user = await _repo.GetUserByResetPasswordCodeAsync(new Guid(resetPasswordModel.ResetPasswordCode));
                if (user is not null)
                {
                    user.Password = Encryptor.Hash(resetPasswordModel.NewPassword);
                    await _repo.DeleteResetPasswordCodeAsync(user);

                    Message = "Your password has been successfully updated.";
                    Status = true;
                }
                else
                {
                    
                    Message = "Invalid Credentials.";
                }
            }
            else
            {
                Message = "Invalid Credentials.";
            }

            ViewBag.Status = Status;
            ViewBag.Message = Message;

            return View(resetPasswordModel);
        }
    }
}
