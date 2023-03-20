using BiographyWebApp.Abstractions;
using BiographyWebApp.Database.DbContexts;
using BiographyWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BiographyWebApp.Database.Repositories
{
    public class AppRepository : IAppRepository
    {
        private readonly IAppDbContext _dbContext;
        public AppRepository(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddUserAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            User? user = await _dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            return user != null;
        }
        public async Task<User?> GetUserByActivationCodeAsync(Guid ActivationCode)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.ActivationCode.Code == ActivationCode);
        }
        public async Task<User?> GetUserByResetPasswordCodeAsync(Guid ResetPasswordCode)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.ResetPasswordCode.Code == ResetPasswordCode);
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }


        public async Task AddResetPasswordCodeForUserAsync(User user)
        {
            if (user.ResetPasswordCode is null)
            {
                user.ResetPasswordCode = new ResetPasswordCode();
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task DeleteResetPasswordCodeAsync(User user)
        {
            _dbContext.ResetPasswordCodes.Remove(user.ResetPasswordCode);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteActivationCodeAndVerifyUserAsync(User user)
        {
            user.IsEmailVerified = true;
            _dbContext.ActivationCodes.Remove(user.ActivationCode);
            await _dbContext.SaveChangesAsync();
        }
    }
}
