using Common.Email;
using Entity; // Assuming MailSettings and WelcomeRequest are in Entity namespace
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting; // ⬅️ Add this for IWebHostEnvironment
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO; // For StreamReader
using System.Threading.Tasks;

namespace Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _env; // Inject IWebHostEnvironment to get wwwroot path
        private FrontendSettings _frontendSettings;

        public MailService(IOptions<MailSettings> mailSettings, IWebHostEnvironment env, IOptions<FrontendSettings> frontendSettings)
        {
            _mailSettings = mailSettings.Value;
            _env = env; // Used for resolving template paths dynamically
            _frontendSettings = frontendSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();

            // Handle attachments if MailRequest has them (your original code had this)
            // if (mailRequest.Attachments != null)
            // {
            //     foreach (var file in mailRequest.Attachments)
            //     {
            //         if (file.Length > 0)
            //         {
            //             using (var ms = new MemoryStream())
            //             {
            //                 file.CopyTo(ms);
            //                 byte[] fileBytes = ms.ToArray();
            //                 builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
            //             }
            //         }
            //     }
            // }

            builder.HtmlBody = mailRequest.Body; // Use the provided HTML body
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using Serilog as configured in appsettings.json)
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw; // Re-throw to be handled by the controller
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task SendWelcomeEmailAsync(WelcomeRequest request)
        {
            // Construct the path dynamically (e.g., if welcome.html is in a "Templates" folder in wwwroot)
            // Replace "D:\\Programming\\Internship\\Common\\Email\\welcome.html" with a configurable path or relative path
            // For example, if welcome.html is in your project's root or a "Templates" folder:
            // string FilePath = Path.Combine(_env.ContentRootPath, "Templates", "welcome.html");
            // Or if it's in wwwroot:
            string FilePath = "D:\\my_code_profile\\newvision\\Dotnet\\PeopleStrong\\PeopleStrong\\Common\\Email\\welcome.html"; // ⬅️ RECOMMENDED: Place welcome.html in a folder like wwwroot/EmailTemplates
          
            // For now, I will use your provided hardcoded path for consistency,
            // but strongly recommend making it dynamic as shown above.
            // string FilePath = "D:\\Programming\\Internship\\Common\\Email\\welcome.html";


            string MailText;
            using (StreamReader str = new StreamReader(FilePath))
            {
                MailText = await str.ReadToEndAsync();
            }

            MailText = MailText.Replace("[username]", request.UserName)
                               .Replace("[email]", request.ToEmail);
            // Replace the hardcoded CLICK HERE link in welcome.html with a placeholder
            // that you can then inject the actual login URL into from FrontendSettings
            // For example, if welcome.html has `[login_link_placeholder]`
            // MailText = MailText.Replace("[login_link_placeholder]", loginLink);


            var mailRequest = new MailRequest
            {
                ToEmail = request.ToEmail,
                Subject = $"Welcome {request.UserName}",
                Body = MailText
            };

            await SendEmailAsync(mailRequest);
        }
    }
}
