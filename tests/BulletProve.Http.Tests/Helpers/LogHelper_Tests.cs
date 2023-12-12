using BulletProve.Http.Configuration;
using BulletProve.Http.Helpers;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace BulletProve.Http.Tests.Helpers
{
    /// <summary>
    /// The log helper tests.
    /// </summary>
    public class LogHelper_Tests
    {
        /// <summary>
        /// Tests the create request log with headers.
        /// </summary>
        [Fact]
        public void TestCreateRequestLogWithHeaders()
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost/")
            };
            request.Headers.TryAddWithoutValidation(HeaderNames.Accept, "application/json");
            request.Headers.TryAddWithoutValidation(Constants.BulletProveRequestID, "id");
            using var content = new StringContent("content");
            content.Headers.TryAddWithoutValidation(Constants.BulletProveRequestID, "id2");
            content.Headers.TryAddWithoutValidation(HeaderNames.ContentType, "application/json");
            request.Content = content;

            var httpClient = new HttpClient();
            var configuration = new HttpConfiguration();

            var log = LogHelper.CreateRequestLog(request, httpClient, configuration);

            log.Should().Be("POST https://localhost/, Headers: { Accept: application/json }, Content: content");
        }

        /// <summary>
        /// Tests the create request log without headers.
        /// </summary>
        [Fact]
        public void TestCreateRequestLogWithoutHeaders()
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri("https://localhost/")
            };
            using var content = new StringContent("content");
            request.Content = content;

            var httpClient = new HttpClient();
            var configuration = new HttpConfiguration();

            var log = LogHelper.CreateRequestLog(request, httpClient, configuration);
            log.Should().Be("PUT https://localhost/, Content: content");
        }

        /// <summary>
        /// Tests the create request log without content.
        /// </summary>
        [Fact]
        public void TestCreateRequestLogWithoutContent()
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://localhost/asd")
            };

            var httpClient = new HttpClient();
            var configuration = new HttpConfiguration();

            var log = LogHelper.CreateRequestLog(request, httpClient, configuration);
            log.Should().Be("GET https://localhost/asd");
        }

        /// <summary>
        /// Tests the create request log with not loggable content.
        /// </summary>
        [Fact]
        public void TestCreateRequestLogWithNotLoggableContent()
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("https://localhost/asd")
            };
            using var stream = new MemoryStream();
            using var content = new StreamContent(stream);

            var httpClient = new HttpClient();
            var configuration = new HttpConfiguration();

            var log = LogHelper.CreateRequestLog(request, httpClient, configuration);
            log.Should().Be("DELETE https://localhost/asd");
        }

        /// <summary>
        /// Tests the create response log.
        /// </summary>
        [Fact]
        public void TestCreateResponseLog()
        {
            using var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            var configuration = new HttpConfiguration();

            var log = LogHelper.CreateResponseLog(response, 250, configuration);
            log.Should().Be("404 (Not Found), 250 ms");
        }

        /// <summary>
        /// Tests the create response log with headers.
        /// </summary>
        [Fact]
        public void TestCreateResponseLogWithHeaders()
        {
            using var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized
            };
            response.Headers.TryAddWithoutValidation(HeaderNames.Accept, "application/json");
            var configuration = new HttpConfiguration();

            var log = LogHelper.CreateResponseLog(response, 300, configuration);
            log.Should().Be("401 (Unauthorized), 300 ms, Headers: { Accept: application/json }");
        }

        /// <summary>
        /// Tests the create response log with headers and content.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        [Theory]
        [InlineData(MediaTypeNames.Application.Json)]
        [InlineData(MediaTypeNames.Text.Xml)]
        [InlineData(MediaTypeNames.Text.Plain)]
        public void TestCreateResponseLogWithHeadersAndContent(string mediaType)
        {
            using var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized
            };
            response.Headers.TryAddWithoutValidation(HeaderNames.Accept, "application/json");
            using var content = new StringContent("{\"a\":12}", Encoding.UTF8, mediaType);
            response.Content = content;
            var configuration = new HttpConfiguration();

            var log = LogHelper.CreateResponseLog(response, 300, configuration);
            log.Should().Be("401 (Unauthorized), 300 ms, Headers: { Accept: application/json }, Content: {\"a\":12}");
        }

        /// <summary>
        /// Tests the create response log with not loggable content.
        /// </summary>
        [Fact]
        public void TestCreateResponseLogWithNotLoggableContent()
        {
            using var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            using var stream = new MemoryStream();
            using var content = new StreamContent(stream);
            response.Content = content;
            var configuration = new HttpConfiguration();

            var log = LogHelper.CreateResponseLog(response, 300, configuration);
            log.Should().Be("200 (OK), 300 ms");
        }
    }
}
