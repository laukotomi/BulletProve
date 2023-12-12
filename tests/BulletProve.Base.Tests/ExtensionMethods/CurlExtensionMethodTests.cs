using BulletProve.Exceptions;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace BulletProve.Tests.ExtensionMethods
{
    /// <summary>
    /// The curl extension method tests.
    /// </summary>
    public class CurlExtensionMethodTests
    {
        /// <summary>
        /// The url.
        /// </summary>
        private const string Url = "https://localhost:8888/";

        /// <summary>
        /// Tests the request method.
        /// </summary>
        /// <param name="methodString">The method string.</param>
        /// <returns>A Task.</returns>
        [Theory]
        [InlineData("POST")]
        [InlineData("GET")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        public async Task TestRequestMethod(string methodString)
        {
            using var request = new HttpRequestMessage(new HttpMethod(methodString), Url);

            var curl = await request.ToCurlAsync();

            curl.Should().Be($"curl -X {methodString} '{Url}'");
        }

        /// <summary>
        /// Tests the headers.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestHeaders()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, Url);
            request.Headers.Add(HeaderNames.Accept, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Token");

            var curl = await request.ToCurlAsync();

            curl.Should().Contain("-H 'Accept: application/json'");
            curl.Should().Contain("-H 'Authorization: Bearer Token'");
        }

        /// <summary>
        /// Tests the content headers.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestContentHeaders()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, Url);
            using var content = new StringContent("something");
            request.Content = content;

            var curl = await request.ToCurlAsync();

            curl.Should().Contain("-H 'Content-Type: text/plain; charset=utf-8'");
        }

        /// <summary>
        /// Tests the string content.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestStringContent()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, Url);
            using var content = new StringContent("'something'");
            request.Content = content;

            var curl = await request.ToCurlAsync();

            curl.Should().Contain("--data-raw ''\\''something'\\'''");
        }

        /// <summary>
        /// Tests the form url encoded content.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestFormUrlEncodedContent()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, Url);
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Key", "Value" },
                { "Key2", "Value2" }
            });
            request.Content = content;

            var curl = await request.ToCurlAsync();

            curl.Should().Contain("-H 'Content-Type: application/x-www-form-urlencoded'");
            curl.Should().Contain("--data-urlencode 'Key=Value'");
            curl.Should().Contain("--data-urlencode 'Key2=Value2'");
        }

        /// <summary>
        /// Tests the invalid content type.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestInvalidContentType()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, Url);
            using var content = new MultipartFormDataContent();
            request.Content = content;

            var act = request.ToCurlAsync;
            await act.Should().ThrowAsync<BulletProveException>();
        }

        /// <summary>
        /// Tests the content lenght header excluded.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestContentLenghtHeaderExcluded()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, Url);
            using var content = new StringContent("something");
            request.Content = content;
            request.Content.Headers.TryAddWithoutValidation(HeaderNames.ContentLength, "100");
            request.Headers.TryAddWithoutValidation(HeaderNames.ContentLength, "100");

            var curl = await request.ToCurlAsync();

            curl.Should().NotContain("-H 'Content-Length:");
        }
    }
}
