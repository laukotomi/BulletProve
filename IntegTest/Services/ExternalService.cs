using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IntegTest.Services
{
    public interface IExternalService
    {
        Task<bool> IsUserCorrectAsync(string username, CancellationToken cancellationToken);
    }

    public class ExternalService : IExternalService
    {
        private readonly HttpClient _httpClient;

        public ExternalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsUserCorrectAsync(string username, CancellationToken cancellationToken)
        {
            var result = await _httpClient.GetAsync("https://google.com");
            if (result != null)
            {
                return true;
            }

            return false;
        }
    }
}
