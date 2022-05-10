using LTest.Mocks.ResponseCache;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LTest.Mocks
{
    /// <summary>
    /// Default message handler.
    /// </summary>
    public class DefaultMessageHandler<T> : DelegatingHandler
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ResponseCacheService<T> _reponseCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseCacheService{T}"/> class.
        /// </summary>
        /// <param name="reponseCache"></param>
        public DefaultMessageHandler(ResponseCacheService<T> reponseCache)
        {
            _reponseCache = reponseCache;
        }

        /// <summary>
        /// Runs when respose of the request is missing. By default request is sent by HttpClient.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual Response AddMissingResponse(HttpRequestMessage request)
        {
            var result = _httpClient.Send(request);
            return CreateResponseObject(result);
        }

        /// <summary>
        /// Send request override.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var req = CreateRequestObject(request);
            var serializedRequest = SerializeRequestForHash(req);
            var hash = CalculateHashKey(serializedRequest);

            var response = _reponseCache.GetOrAdd(hash, req, () => AddMissingResponse(CopyRequest(request)));
            cancellationToken.ThrowIfCancellationRequested();

            var responseMessage = CreateResponseMessage(request, response);
            return Task.FromResult(responseMessage);
        }

        private HttpResponseMessage CreateResponseMessage(HttpRequestMessage request, Response response)
        {
            var responseMessage = new HttpResponseMessage
            {
                Content = DeserializeContent(response.Content),
                ReasonPhrase = response.ReasonPhrase,
                RequestMessage = request,
                StatusCode = (HttpStatusCode)response.StatusCode,
                Version = JsonConvert.DeserializeObject<Version>(response.Version)
            };

            CopyHeaders(JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, IEnumerable<string>>>>(response.Headers), responseMessage.Headers);
            CopyHeaders(JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, IEnumerable<string>>>>(response.TrailingHeaders), responseMessage.TrailingHeaders);

            return responseMessage;
        }

        /// <summary>
        /// Serializes request for hashing. By default Method, Uri and Content are serialized. Headers are not.
        /// </summary>
        /// <param name="request">Request.</param>
        protected virtual string SerializeRequestForHash(Request request)
        {
            return JsonConvert.SerializeObject(new Request
            {
                Content = request.Content,
                Method = request.Method,
                Uri = request.Uri
            });
        }

        /// <summary>
        /// Serializes http content.
        /// </summary>
        /// <param name="content">Content.</param>
        protected virtual Content SeriliazeContent(HttpContent content)
        {
            if (content == null)
            {
                return null;
            }

            return new Content
            {
                Data = content.ReadAsStringAsync().GetAwaiter().GetResult(),
                Headers = JsonConvert.SerializeObject(content.Headers),
                Type = JsonConvert.SerializeObject(content.GetType())
            };
        }

        /// <summary>
        /// Deserialize content.
        /// </summary>
        /// <param name="content">Content.</param>
        protected virtual HttpContent DeserializeContent(Content content)
        {
            if (content == null)
            {
                return null;
            }

            var con = new StringContent(content.Data);
            CopyHeaders(JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, IEnumerable<string>>>>(content.Headers), con.Headers);
            return con;
        }

        /// <summary>
        /// Creates response object from HttpResponseMessage.
        /// </summary>
        /// <param name="response">Response.</param>
        protected virtual Response CreateResponseObject(HttpResponseMessage response)
        {
            return new Response
            {
                Content = SeriliazeContent(response.Content),
                Headers = JsonConvert.SerializeObject(response.Headers),
                StatusCode = (int)response.StatusCode,
                ReasonPhrase = response.ReasonPhrase,
                TrailingHeaders = JsonConvert.SerializeObject(response.TrailingHeaders),
                Version = JsonConvert.SerializeObject(response.Version)
            };
        }

        /// <summary>
        /// Creates request object from HttpRequestMessage.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns></returns>
        protected virtual Request CreateRequestObject(HttpRequestMessage request)
        {
            return new Request
            {
                Method = request.Method.Method,
                Uri = request.RequestUri.ToString(),
                Content = SeriliazeContent(request.Content),
                Headers = JsonConvert.SerializeObject(request.Headers)
            };
        }

        private string CalculateHashKey(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private void CopyHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> sourceHeaders, HttpHeaders destinationHeaders)
        {
            foreach (var item in sourceHeaders)
            {
                destinationHeaders.TryAddWithoutValidation(item.Key, item.Value);
            }
        }

        // Needed because a request can not be sent twice.
        private HttpRequestMessage CopyRequest(HttpRequestMessage request)
        {
            var req = new HttpRequestMessage(request.Method, request.RequestUri);
            CopyHeaders(request.Headers, req.Headers);
            req.Content = request.Content;

            return req;
        }
    }
}