using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HttpClientFactory.Services
{
    public class CRUDService: IIntegrationService
    {
        private readonly ILogger<CRUDService> _logger;

        public CRUDService(ILogger<CRUDService> logger)
        {
            _logger = logger;
        }

        public async Task Run()
        {
            _logger.LogInformation("hello from CRUDService");
        }
    }
}