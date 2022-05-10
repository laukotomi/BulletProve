using LTest.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace LTest.Mocks.ResponseCache
{
    /// <summary>
    /// Response cache.
    /// </summary>
    public class ResponseCacheService<T>
    {
        private readonly ITestLogger _logger;
        private readonly string _responseCacheDir;
        private readonly bool _enableAddingRequest;
        private readonly bool _enableGeneratingResponse;
        private ConcurrentDictionary<string, Data> _cache = new();
        private bool _inited;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseCacheService{T}"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="configuration">Configuration.</param>
        public ResponseCacheService(ITestLogger logger, IConfiguration configuration)
        {
            _logger = logger;
            var settings = configuration.GetSection("ResponseCache")?.Get<ResponseCacheSettings>() ?? new ResponseCacheSettings();

            _enableAddingRequest = settings.EnableAddingRequest;
            _enableGeneratingResponse = settings.EnableGeneratingResponse;
            _responseCacheDir = settings.ResponseCacheDir;

            if (!string.IsNullOrEmpty(_responseCacheDir))
            {
                _responseCacheDir = _responseCacheDir.Replace("{outputdir}", Directory.GetCurrentDirectory(), StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Returns the response from cache or adds it to the cache.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="request">Request.</param>
        /// <param name="addAction">Add action.</param>
        public Response GetOrAdd(string key, Request request, Func<Response> addAction)
        {
            EnsureInited();

            var data = _cache.GetOrAdd(key, _ =>
            {
                return new Data
                {
                    Request = request
                };
            });

            if (data.Response == null)
            {
                if (_enableAddingRequest)
                {
                    TrySave();
                }

                if (!_enableGeneratingResponse)
                {
                    throw new InvalidOperationException($"Could not add {key} for request {request.Method} {request.Uri} to {typeof(T).Name} because response cache is set to read-only");
                }

                data.Response = addAction();
                TrySave();

                _logger.Info($"MockStore: Registered {key}");
            }
            else
            {
                _logger.Info($"MockStore: Found {key}");
            }

            return data.Response;
        }

        private void TrySave()
        {
            if (!string.IsNullOrEmpty(_responseCacheDir))
            {
                var path = Path.Combine(_responseCacheDir, $"{typeof(T).Name}.json");
                File.WriteAllText(path, JsonConvert.SerializeObject(_cache, Formatting.Indented));
            }
        }

        private void EnsureInited()
        {
            if (_inited)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_responseCacheDir))
            {
                if (!Directory.Exists(_responseCacheDir))
                {
                    throw new InvalidOperationException($"Response cache directory does not exist at {_responseCacheDir}");
                }

                var path = Path.Combine(_responseCacheDir, $"{typeof(T).Name}.json");
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    if (!string.IsNullOrEmpty(text))
                    {
                        _cache = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Data>>(text);
                    }
                }
            }
            else
            {
                _logger.Warning($"{nameof(ResponseCacheSettings.ResponseCacheDir)} was not set");
            }

            _inited = true;
        }
    }
}