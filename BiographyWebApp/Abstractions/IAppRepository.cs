using BiographyWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BiographyWebApp.Abstractions
{
    public interface IAppRepository
    {
        Task AddUserAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserByActivationCodeAsync(Guid ActivationCode);
        Task<User?> GetUserByResetPasswordCodeAsync(Guid ResetPasswordCode);
        Task<User?> GetUserByEmailAsync(string email);
        Task DeleteActivationCodeAndVerifyUserAsync(User user);
        Task DeleteResetPasswordCodeAsync(User user);
        Task AddResetPasswordCodeForUserAsync(User user);
    }
}
