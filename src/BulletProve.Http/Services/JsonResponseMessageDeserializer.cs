using System.Text.Json;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// The json response message deserializer.
    /// </summary>
    public class JsonResponseMessageDeserializer : ResponseMessageDeserializer
    {
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponseMessageDeserializer"/> class.
        /// </summary>
        /// <param name="serializerOptions">The serializer options.</param>
        public JsonResponseMessageDeserializer(JsonSerializerOptions serializerOptions)
        {
            _serializerOptions = serializerOptions;
        }

        /// <inheritdoc/>
        protected override TResponse Deserialize<TResponse>(string responseMessage)
        {
            return JsonSerializer.Deserialize<TResponse>(responseMessage, _serializerOptions)!;
        }
    }
}
