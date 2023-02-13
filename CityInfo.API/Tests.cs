using CityInfo.API.Services;
using Xunit;

namespace CityInfo.API
{
    public class Tests
    {
        private readonly IMailService _mailService;
        public Tests(IMailService mailService)
        {
            _mailService = mailService;
        }
        [Fact]
        public void x()
        {
            _mailService.send("sdf", "sdfsd");
            
        }
    }
}
