namespace BiographyWebApp.Models
{
    public class ResetPasswordCode
    {
        public int Id { get; set; }
        public Guid Code { get; set; } = Guid.NewGuid();


        public ICollection<User> Users { get; set; }
    }
}
