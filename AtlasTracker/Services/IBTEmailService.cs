namespace AtlasTracker.Services
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string emailTo, string subject, string htmlMessage);
    }
}
