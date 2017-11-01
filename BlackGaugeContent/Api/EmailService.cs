using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Bgc.Api
{
	public class EmailService
	{
		private readonly string _apiKey;

		public EmailService(string apiKey)
		{
			_apiKey = apiKey;
		}

		public async Task SendAsync()
		{
			await ConfigSendGridasync();
		}

		// Use NuGet to install SendGrid (Basic C# client lib) 
		private async Task ConfigSendGridasync()
		{
			var client = new SendGridClient(_apiKey);
			var @from = new EmailAddress("emhryr@gmail.com", "Emhryr");
			var to = new EmailAddress("Emhryr@gmail.com", "Brat Wiekszy");
			var subject = "email service test";
			var plainTextContent = "do not respond.";

			var message = MailHelper.CreateSingleEmail(@from, to, subject, plainTextContent, plainTextContent);
			await client.SendEmailAsync(message);
		}
	}
}
