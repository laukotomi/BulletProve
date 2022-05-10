using IntegTest.Services;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationTests.Mocks
{
    public class ExternalServiceMock : IExternalService
    {
        public Task<bool> IsUserCorrectAsync(string username, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
