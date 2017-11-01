using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Bgc.Services
{
	// This class is used by the application to send email for account confirmation and password reset.
	// For more details see https://go.microsoft.com/fwlink/?LinkID=532713
	public class EmailSender : IEmailSender
	{
		private AuthMessageSenderOptions _options;

		public EmailSender(IOptions<AuthMessageSenderOptions> options)
		{
			_options = options.Value;
		}


		public Task SendEmailAsync(string email, string subject, string message)
		{
			return Execute(subject, message, email);
		}

		private Task Execute(string subject, string message, string email)
		{
			var client = new SendGridClient(_options.SendGridApiKey);
			var msg = new SendGridMessage()
			{
				From = new EmailAddress("emhryr@gmail.com", "Emhryr"),
				Subject = subject,
				PlainTextContent = message,
				HtmlContent = message
			};
			msg.AddTo(new EmailAddress(email));
			return client.SendEmailAsync(msg);
		}
	}
}
