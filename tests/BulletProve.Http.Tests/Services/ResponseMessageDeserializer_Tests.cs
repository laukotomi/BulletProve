using BulletProve.Http.Services;
using FluentAssertions;

namespace BulletProve.Http.Tests.Services
{
    /// <summary>
    /// The response message deserializer tests.
    /// </summary>
    public class ResponseMessageDeserializer_Tests
    {
        private readonly Deserializer _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMessageDeserializer_Tests"/> class.
        /// </summary>
        public ResponseMessageDeserializer_Tests()
        {
            _sut = new Deserializer();
        }

        /// <summary>
        /// Tests the get response object async http response message.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestGetResponseObjectAsyncHttpResponseMessage()
        {
            using var response = new HttpResponseMessage();
            var result = await _sut.GetResponseObjectAsync<HttpResponseMessage>(response);
            result.Should().Be(response);
        }

        /// <summary>
        /// Tests the get response object async empty response.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestGetResponseObjectAsyncEmptyResponse()
        {
            using var response = new HttpResponseMessage();
            var result = await _sut.GetResponseObjectAsync<EmptyResponse>(response);
            result.Should().NotBeNull().And.BeOfType<EmptyResponse>();
        }

        /// <summary>
        /// Tests the get response object async empty response not empty content.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestGetResponseObjectAsyncEmptyResponseNotEmptyContent()
        {
            using var response = new HttpResponseMessage();
            using var content = new StringContent("asd");
            response.Content = content;

            var act = async () => await _sut.GetResponseObjectAsync<EmptyResponse>(response);
            await act.Should().ThrowAsync<NotImplementedException>();
        }

        /// <summary>
        /// Tests the get response object async string response.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestGetResponseObjectAsyncStringResponse()
        {
            using var response = new HttpResponseMessage();
            using var content = new StringContent("asd");
            response.Content = content;

            var result = await _sut.GetResponseObjectAsync<string>(response);
            result.Should().Be("asd");
        }

        /// <summary>
        /// Tests the get response object async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestGetResponseObjectAsync()
        {
            using var response = new HttpResponseMessage();
            using var content = new StringContent("asd");
            response.Content = content;

            var act = async () => await _sut.GetResponseObjectAsync<Response>(response);
            await act.Should().ThrowAsync<NotImplementedException>();
        }

        /// <summary>
        /// The deserializer.
        /// </summary>
        private class Deserializer : ResponseMessageDeserializer
        {
            /// <inheritdoc/>
            protected override TResponse Deserialize<TResponse>(string responseMessage)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The response.
        /// </summary>
        private class Response
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public required string Name { get; set; }
        }
    }
}
