namespace CarRentalSystem.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);
        Task SendDefaultAsync(string subject, string htmlBody);
    }
}