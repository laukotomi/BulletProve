using BulletProve.Http.Configuration;
using BulletProve.Http.Models;
using BulletProve.Http.Services;
using BulletProve.Logging;
using BulletProve.ServerLog;
using BulletProve.Services;
using BulletProve.TestServer;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NSubstitute;
using System.Text.Json;

namespace BulletProve.Http.Tests.Services
{
    /// <summary>
    /// The http request builder_ tests.
    /// </summary>
    public class HttpRequestBuilder_Tests
    {
        protected readonly HttpRequestBuilder _sut;
        protected readonly HttpResponseMessage _response;
        protected readonly IAssertionRunner<MyContent> _assertionRunner;
        protected readonly IAssertionBuilder<MyContent> _assertionBuilder;
        protected readonly IHttpRequestManager _httpRequestManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestBuilder_Tests"/> class.
        /// </summary>
        public HttpRequestBuilder_Tests(Action<ServiceCollection>? servicesCallback = null)
        {
            var linkGenerator = Substitute.For<ILinkGeneratorService>();
            linkGenerator
                .GetRequestUri(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Dictionary<string, string>?>())
                .Returns(x =>
                {
                    var url = $"{x.ArgAt<string>(1)}/{x.ArgAt<string>(0)}";

                    var arg2 = x.ArgAt<Dictionary<string, string>?>(2);
                    if (arg2 != null && arg2.Count > 0)
                        return new Uri($"{url}/{JsonSerializer.Serialize(arg2)}", UriKind.RelativeOrAbsolute);
                    return new Uri(url, UriKind.RelativeOrAbsolute);
                });

            _response = new HttpResponseMessage();
            _response.Content = new StringContent("{\"Name\":\"asd\"}");

            _assertionRunner = Substitute.For<IAssertionRunner<MyContent>>();
            _assertionBuilder = Substitute.For<IAssertionBuilder<MyContent>>();
            _assertionBuilder.BuildAssertionRunner(Arg.Any<IServerScope>()).Returns(_assertionRunner);

            _httpRequestManager = Substitute.For<IHttpRequestManager>();
            _httpRequestManager.ExecuteRequestAsync(Arg.Any<HttpRequestContext>(), Arg.Any<IServerScope>())
                .Returns(_response);

            var services = new ServiceCollection();
            services.AddSingleton<LabelGeneratorService>();
            services.AddSingleton<DisposableCollector>();
            services.AddSingleton<ITestLogger, TestLogger>();
            services.AddSingleton<ScopeProvider>();
            services.AddSingleton(linkGenerator);
            services.AddSingleton(_httpRequestManager);
            services.AddSingleton<IServerLogCollector, ServerLogCollector>();
            services.AddSingleton(new ServerConfigurator());
            services.AddSingleton(new HttpConfiguration());
            services.AddTransient(x => _assertionBuilder);
            servicesCallback?.Invoke(services);

            var scope = new ServerScope(services.BuildServiceProvider(), new());

            var context = new LinkGeneratorContext(HttpMethod.Post, "controller", "action");
            _sut = new HttpRequestBuilder(context, scope);
        }

        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            _sut.Context.Should().NotBeNull();
        }

        /// <summary>
        /// Tests the set headers.
        /// </summary>
        [Fact]
        public void TestSetHeaders()
        {
            _sut.SetHeaders(x => x.TryAddWithoutValidation(HeaderNames.Authorization, "Bearer: token"));
            _sut.Context.Request.Headers.Should().HaveCount(1);
            var header = _sut.Context.Request.Headers.First();
            header.Key.Should().Be(HeaderNames.Authorization);
            header.Value.First().ToString().Should().Be("Bearer: token");
        }

        /// <summary>
        /// Tests the set bearer token.
        /// </summary>
        [Fact]
        public void TestSetBearerToken()
        {
            _sut.SetBearerToken("token");
            _sut.Context.Request.Headers.Authorization!.Scheme.Should().Be("Bearer");
            _sut.Context.Request.Headers.Authorization!.Parameter.Should().Be("token");
        }

        /// <summary>
        /// Tests the set json content.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestSetJsonContent()
        {
            _sut.SetJsonContent(new MyContent
            {
                Name = "Test",
            });

            _sut.Context.Request.Content.Should().BeOfType<StringContent>();
            var contentStr = await _sut.Context.Request.Content!.ReadAsStringAsync();
            contentStr.Should().Be("{\"Name\":\"Test\"}");
        }

        /// <summary>
        /// Tests the set content.
        /// </summary>
        [Fact]
        public void TestSetContent()
        {
            using var content = new StringContent("asd");
            _sut.SetContent(content);
            _sut.Context.Request.Content.Should().Be(content);
        }

        /// <summary>
        /// Tests the configure http request.
        /// </summary>
        [Fact]
        public void TestConfigureHttpRequest()
        {
            _sut.ConfigureHttpRequest(x => x.Method = HttpMethod.Delete);
            _sut.Context.Request.Method.Should().Be(HttpMethod.Delete);
        }

        /// <summary>
        /// Tests the set query parameters with curl.
        /// </summary>
        [Fact]
        public void TestSetQueryParametersWithCurl()
        {
            _sut.SetQueryParameters(x => x.Add("asd", "dsa"));
            _sut.ExportToCurl(out var curl);
            curl.Should().Contain("{\"asd\":\"dsa\"}");
        }

        /// <summary>
        /// Tests the set label empty.
        /// </summary>
        [Fact]
        public void TestSetLabelEmpty()
        {
            var act = () => _sut.SetLabel(string.Empty);
            act.Should().Throw<ArgumentNullException>();
        }

        /// <summary>
        /// Tests the set label.
        /// </summary>
        [Fact]
        public void TestSetLabel()
        {
            _sut.SetLabel("label");
            _sut.Context.Label.Should().Be("label");
        }

        /// <summary>
        /// Tests the export to curl.
        /// </summary>
        [Fact]
        public void TestExportToCurl()
        {
            _sut.ExportToCurl(out var curl);
            curl.Should().Be("curl -X POST 'controller/action'");
        }

        /// <summary>
        /// Tests the add allowed server log event.
        /// </summary>
        [Fact]
        public void TestAddAllowedServerLogEvent()
        {
            _sut.AddAllowedServerLogEvent(x => x.Message.Length == 3);
            var isAllowed = _sut.Context.ServerLogInspector.IsAllowed(new ServerLogEvent("cat", LogLevel.Error, new(), "asd", null, null));
            isAllowed.Should().BeTrue();
        }


        /// <summary>
        /// Executes the request async with response type.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestExecuteRequestAsync()
        {
            bool invoked = false;

            await _sut.ExecuteRequestAsync<MyContent>(x =>
            {
                x.Should().Be(_assertionBuilder);
                invoked = true;
            });

            invoked.Should().BeTrue();
            await _httpRequestManager.Received(1).ExecuteRequestAsync(_sut.Context, Arg.Any<IServerScope>());
            _assertionBuilder.Received(1).BuildAssertionRunner(Arg.Any<IServerScope>());
            _assertionRunner.Received(1).RunResponseMessageAssertions(_response);
            _assertionRunner.Received(1).RunServerLogAssertions(Arg.Any<IReadOnlyCollection<ServerLogEvent>>());
            _assertionRunner.Received(1).RunResponseObjectAssertions(Arg.Any<MyContent>());
        }

        /// <summary>
        /// The my content.
        /// </summary>
        public class MyContent
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public required string Name { get; set; }
        }
    }
}
