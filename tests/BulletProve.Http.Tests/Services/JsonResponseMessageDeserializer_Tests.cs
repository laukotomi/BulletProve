using BulletProve.Http.Services;
using FluentAssertions;
using System.Text.Json;

namespace BulletProve.Http.Tests.Services
{
    /// <summary>
    /// The json response message deserializer_ tests.
    /// </summary>
    public class JsonResponseMessageDeserializer_Tests
    {
        /// <summary>
        /// Tests the deserialize.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestDeserialize()
        {
            var sut = new JsonResponseMessageDeserializer(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            using var response = new HttpResponseMessage();
            using var content = new StringContent("{\"asd\":\"dsa\"}");
            response.Content = content;
            var result = await sut.GetResponseObjectAsync<Response>(response);
            result.Should().NotBeNull();
            result.Asd.Should().Be("dsa");
        }

        /// <summary>
        /// The response.
        /// </summary>
        private class Response
        {
            /// <summary>
            /// Gets or sets the asd.
            /// </summary>
            public required string Asd { get; set; }
        }
    }
}
