using BulletProve.Mocks;
using FluentAssertions;
using System.Net;

namespace BulletProve.Tests.Mocks
{
    /// <summary>
    /// The http message handler base tests.
    /// </summary>
    public class HttpMessageHandlerBase_Tests : HttpMessageHandlerBase
    {
        /// <summary>
        /// Tests the create json response message.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestCreateJsonResponseMessage()
        {
            using var request = new HttpRequestMessage();
            int[] content = [2, 3, 4];

            using var response = CreateJsonResponseMessage(request, HttpStatusCode.OK, content);

            response.RequestMessage.Should().Be(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var contentStr = await response.Content.ReadAsStringAsync();
            contentStr.Should().Be("[2,3,4]");
        }

        /// <inheritdoc/>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
