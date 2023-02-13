namespace CityInfo.API.Services
{
    public class CloudMailServices : IMailService
    {



        private readonly string _mailTo = string.Empty;
        private readonly string _mailFrom = string.Empty;

        public CloudMailServices(IConfiguration configuration)
        {
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailFrom = configuration["mailSettings:mailFromAddress"];
        }

        public void send(string subject, string message)
        {
            //send mail output to console window
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}" +
                $", with {nameof(CloudMailServices)}. ");
            Console.WriteLine($"Subject : {subject}");
            Console.WriteLine($"Message : {message}");

        }
    
}
}
