using BiographyWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BiographyWebApp.Abstractions
{
    public interface IAppDbContext
    {
        DbSet<ActivationCode> ActivationCodes { get; set; }
        DbSet<ResetPasswordCode> ResetPasswordCodes { get; set; }
        DbSet<User> Users { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
