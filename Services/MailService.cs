using Common.Email;
using Entity;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting; // Added for IWebHostEnvironment
using Microsoft.Extensions.Logging; // Added for ILogger
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _env; // Injected for path handling
        private FrontendSettings _frontendSettings; // Ensure this is also injected
        private readonly ILogger<MailService> _logger; // Injected for logging

        public MailService(
            IOptions<MailSettings> mailSettings,
            IWebHostEnvironment env,
            IOptions<FrontendSettings> frontendSettings,
            ILogger<MailService> logger) // Add ILogger here
        {
            _mailSettings = mailSettings.Value;
            _env = env;
            _frontendSettings = frontendSettings.Value;
            _logger = logger; // Assign logger
        }

        // Corrected SendEmailAsync to return bool and handle exceptions internally
        public async Task<bool> SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true); // Disconnect after sending

                _logger.LogInformation("Email sent successfully to {ToEmail} with subject {Subject}.", mailRequest.ToEmail, mailRequest.Subject);
                return true; // Successfully sent
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail} with subject {Subject}. Error: {ErrorMessage}", mailRequest.ToEmail, mailRequest.Subject, ex.Message);
                return false; // Failed to send
            }
        }

        // Corrected SendWelcomeEmailAsync to return bool and handle exceptions internally
        public async Task<bool> SendWelcomeEmailAsync(WelcomeRequest request)
        {
            try
            {
                // RECOMMENDED: Place welcome.html in wwwroot/EmailTemplates
                // Example: Your API project -> wwwroot -> EmailTemplates -> welcome.html
                string FilePath = "D:\\my_code_profile\\newvision\\Dotnet\\PeopleStrong\\PeopleStrong\\Common\\Email\\welcome.html";

                // You might need to adjust the path based on where your welcome.html is
                // If Common is a separate library, consider it as an embedded resource
                // For example: FilePath = GetEmbeddedResourceContent("Common.Email.welcome.html");

                if (!System.IO.File.Exists(FilePath))
                {
                    _logger.LogError("Welcome email template not found at: {FilePath}", FilePath);
                    return false;
                }

                string MailText;
                using (StreamReader str = new StreamReader(FilePath))
                {
                    MailText = await str.ReadToEndAsync();
                }

                MailText = MailText.Replace("[username]", request.UserName)
                                   .Replace("[email]", request.ToEmail);
                MailText = MailText.Replace("[LOGIN_LINK]", _frontendSettings.LoginUrl); // Ensure this is also working

                var mailRequest = new MailRequest
                {
                    ToEmail = request.ToEmail,
                    Subject = $"Welcome {request.UserName}",
                    Body = MailText
                };

                // Delegate sending to SendEmailAsync and return its result
                return await SendEmailAsync(mailRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to prepare and send welcome email for {ToEmail}. Error: {ErrorMessage}", request.ToEmail, ex.Message);
                return false; // Preparation or sending failed
            }
        }

        // ⬅️ REMOVED the duplicate explicit interface implementations that threw NotImplementedException
        // Task<bool> IMailService.SendEmailAsync(MailRequest mailRequest) { throw new NotImplementedException(); }
        // Task<bool> IMailService.SendWelcomeEmailAsync(WelcomeRequest request) { throw new NotImplementedException(); }
    }
}
