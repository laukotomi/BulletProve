namespace BulletProve.Http.Services
{
    /// <summary>
    /// The response message deserializer.
    /// </summary>
    public abstract class ResponseMessageDeserializer
    {
        /// <summary>
        /// Gets the response object.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>A TResponse.</returns>
        public async Task<TResponse> GetResponseObjectAsync<TResponse>(HttpResponseMessage response)
            where TResponse : class
        {
            var responseType = typeof(TResponse);
            if (responseType == typeof(HttpResponseMessage))
            {
                return (response as TResponse)!;
            }

            var responseMessage = await response.Content.ReadAsStringAsync();
            var responseObject = DeserializeReponseMessage<TResponse>(responseMessage, responseType);
            response.Dispose();

            return responseObject;
        }

        /// <summary>
        /// Deserialize reponse message.
        /// </summary>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="responseType">The response type.</param>
        /// <returns>A TResponse.</returns>
        private TResponse DeserializeReponseMessage<TResponse>(string responseMessage, Type responseType)
            where TResponse : class
        {
            if (responseType == typeof(EmptyResponse) && string.IsNullOrWhiteSpace(responseMessage))
            {
                return (EmptyResponse.Value as TResponse)!;
            }
            else if (responseType == typeof(string))
            {
                return (responseMessage as TResponse)!;
            }

            return Deserialize<TResponse>(responseMessage);
        }

        /// <summary>
        /// Deserializes the response message.
        /// </summary>
        /// <param name="responseMessage">The response message.</param>
        /// <returns>A TResponse.</returns>
        protected abstract TResponse Deserialize<TResponse>(string responseMessage) where TResponse : class;
    }
}
