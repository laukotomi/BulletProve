# LTest

LTest is an extension to the Microsoft's ASP.NET Core integration test package with useful features.

## Features

### Better management of test servers

By default if you follow the instructions on Microsoft's Integration tests in ASP.NET Core website and use the xUnit's IClassFixture interface for every test class then xUnit will create a new test server for every test class you defines. Initialization of the servers is one of the longest part of running integration tests and doing that many times might not be the best solution.

This framework uses ICollectionFixture to initialize the test server only once and run all of your tests on that instance which can save up a lot of time.

An example test server class looks like this:

    [DefaultTestServer] // This attribute lets the framework find the default test server without any registration.
    public class TestServer : TestServerBase<Startup> // Startup is the startup class of the application that is being tested.
    {
        protected override void Configure(LTestConfiguration config)
        {
            // Here you can change some framework configuration if needed.
        }

        protected override void ConfigureTestServices(IServiceCollection services)
        {
            // Here you can register your own services, mocks and framework services.
        }
    }

### Easily define test servers

You can define many test servers for different purposes. For example if you need a server with special mocks.

However instead of using a new server it might be better to write configurable mocks and reuse the existing default server.

    public class AnotherTestServer : TestServerBase<Startup> // Startup is the startup class of the application that is being tested.
    {
        protected override void Configure(LTestConfiguration config)
        {
            // Here you can change some framework configuration if needed.
        }

        protected override void ConfigureTestServices(IServiceCollection services)
        {
            // Here you can register your own services, mocks and framework services.
        }
    }

In this case you will need to use the [TestServer(typeof(AnotherTestServer))] attribute on your test class or test method or pass the test server's type as an attribute of your test method (xUnit theory).

### Overwrite configuration for testing

Just add integrationtestsettings.json to the root of the integration test project, enable it to be copied to the output directory in the properties panel and you can overwrite the default configuration of your project.

You can also specify your own configuration files in the framework configuration.

### Hooks

You can define integration test lifecycle hooks by implementing lightweight interfaces and registering the class into the DI container.

Hook interfaces:
- IBeforeTestHook: runs before every test.
- IAfterTestHook: runs after every test.
- IAfterServerStarted: runs only once, when the test server was started.
- IResetSingletonHook: a special hook to reset singletons. It will be invoked only for the second and after the seconds tests before running the test logic. These hooks will be registered into the DI automatically.
- IBeforeHttpRequestHook (for LTest.Http): runs before every http requests.
- IAfterHttpRequestHook (for LTest.Http): runs after every http requests.

Example hook:

    public class SeedDatabaseHook : IBeforeTestHook // Will be run before each test.
    {
        private readonly Seeder _seeder; // Seeder is a simple service to seed the database.
        private readonly ITestLogger _testLogger;

        public SeedDatabaseHook(Seeder seeder, ITestLogger testLogger)
        {
            _seeder = seeder;
            _testLogger = testLogger;
        }

        public async Task BeforeTestAsync()
        {
            // StopwatchHelper class can be used to measure the execution of an action.
            var elapsedMs = await StopwatchHelper.MeasureAsync(async () => 
            {
                await _seeder.SeedAsync();
            });

            _testLogger.LogInformation($"Seed done ({elapsedMs} ms)");
        }
    }

### Logging

While the test is running you have the ability to log useful information in the Test Explorer's output using ITestLogger (see previous hook example). It is a wrapper around xUnit's ITestOutputHelper.

Also the server logs are added to the output so you can see what happened exactly. By default the unexpected logs (see LogSniffer later) and the logs which level is at least information and category name starts with the namespace (or project name) of the startup class will be added.

The first letter of a log line indicates the log level (D = Debug, I = Information, W = warning, E = error).

Example logs:

    I: Starting server 'TestServer'
    I: Server started (192 ms)
    
    I: DB cleaned (902 ms)
    I: Seed done (503 ms)
    I: POST /Auth, Content: {"Username":"Admin","Password":"Admin"}
    I:   BasicAuthentication was not authenticated. Failure message: No auth header!
    I:   200 (OK), 177 ms, Content: 08b88215-aeb7-4910-8a01-131c1c0607d9
    I:   200 StatusCode checked
    I: Request '#1' executed
    
    I: Test finished

This can be overwritten in the test server's configuration like this:

    protected override void Configure(LTestConfiguration config)
    {
        config.MinimumLogLevel = LogLevel.Debug;
        config.ServerLogFilter.Clear();
        config.ServerLogFilter.Add(categoryName => categoryName.StartsWith("Anything"));
    }

There are also scopes in the logs. Scoped logs will be indented with spaces.

### LogSniffer

LogSniffer is a feature that can read the application logs and decide if that log was expected or not. The idea of this feature come up when an application that used Entity Framework logged many warning messages, because the queries were not formed correctly.

By default when LogSniffer is enabled only log levels of information and below are accepted. The rule is that you have to specify expected logs at the start of the test or before sending a request.

This can be done in a test method like this:

    LTestServices.LogSniffer.ExpectedLogs.Add(x => x.Message == "Wrong username or password");

Expected logs will be reset after each test. If you want to add global expected logs (or make every log expected) you can do it in the configuration of the test server:

    config.LogSniffer.DefaultExpectedEvents.Add(x => x.Exception == null);

If you want to reset expected logs to the defaults manually in test then call the LTestServices.LogSniffer.ExpectedLogs.Reset() method or wrap it in a using block.

    using (LTestServices.LogSniffer.ExpectedLogs.Add(x => x.Message == "Wrong username or password"))
    {
    ...
    }

### Mocking

Mocking is as easy as registering new classes for interfaces in the test server's ConfigureTestServices method.

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        // Mocks
        services.AddTransient<IExternalService, ExternalServiceMock>();
    }

Don't mock too much! The less code you mock, the more code you will test. For example if you have a class that uses HttpClient to send HTTP requests then you can mock the responses with a HttpMessageHandler:

    protected override void Configure(LTestConfiguration config)
    {
        // Mocks
        services.AddHttpClient<IEmailClient, EmailClient>()
            .ConfigurePrimaryHttpMessageHandler<EmailClientMessageHandler>();

        services.AddSingleton<EmailClientMessageHandler>();
    }

EmailClientMessageHandler should implement HttpMessageHandler's SendAsync method. LTest also has a helper class LTestHttpMessageHandlerBase to derive from which defines a useful CreateJsonResponseMessage method.

With mocks you can easily verify sent messages, requests. If a mock is singleton then don't forget to implement IResetSingletonHook like the built in LTestMockSender if you need to reset it before each tests.

### Export to curl

There is a basic extension method to convert HttpRequestMessage to curl so while debugging you can export a request to Postman. You can use it on any HttpRequestMessage class, so while creating a request and also while mocking a response.

    var builder = _authController
        .CreateFor(x => x.LoginAsync)
        .SetJsonContent(new AuthController.LoginCommand
        {
            Username = TestConstants.AdminUsername,
            Password = TestConstants.AdminPassword,
        });

    var curl = await builder.Request.ToCurlAsync();

    var token = await builder
        .ExecuteSuccessAsync<string>();

### Packages

Right now there are two nuget packages that have more useful extensions for special cases:

- LTest.Http: helps you generate http requests easily.
- LTest.EfCore: helps you reset your database for every test.

Packages usually needs to register classes into the DI, so you need to reference them in the test server's ConfigureTestServices method:

        protected override void ConfigureTestServices(IServiceCollection services)
        {
            ...

            services.AddLTestHttp();
            services.AddLTestEFCore();
        }

## LTest.Http package

### Http request builder

Http request builder is a feature with which you can create HTTP requests easily. You don't have to bother with urls, because the built-in ASP.NET link generator will generate it using the action name. It also uses fluent api so you can write requests like this:

    var authController = LTestServices.GetHttpRequestBuilder<AuthController>(); // Gets the http request builder for AuthController
    var token = await authController
        .CreateFor(x => x.LoginAsync) // You have to specify the action method or method name.
        .SetJsonContent(new AuthController.LoginCommand // Use Set* methods to build the request
        {
            Username = username,
            Password = password
        })
        .ExecuteSuccessAsync<string>(); // Use Assert* methods to start asserting the request or Execute* methods to use built in assertions.

The Assert* and the Execute* methods can have generic types which controls the returned object after sending the request:

 - If no type is specified then HttpResponseMesssage will be returned.
 - If type is string then the responses content will be read as string and returned.
 - If a class is specified then the content of the response will be deserialized to that class (only json is supported). Additionally there is a check that the class must contain all of the properties of the returned json string. This way it can be avoided that a json response is treated as a wrong class.

## LTest.EfCore package

### CleanDatabaseHook

With this hook you can clean your database before each tests, so they won't conflict each other. **The database will be cleaned, so use it carefully!** Usually it is adviced to use a dedicated database for integration tests only. Cleaning the database before the test is useful because you can see the state exactly after a test was run.

To use the hook register it like this:

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        // Hooks
        services.AddCleanDatabaseHook<AppDbContext>();
    }

Don't forget to seed the database if needed.

## Installation

- Create a new 'xUnit Test Project' in Visual Studio.
- Add the needed LTest* nuget packages (LTest.Http and LTest.EfCore includes LTest)
- Create the following two files.

### Add xunit.runner.json

In order to prevent running integration tests in parallel create the following *xunit.runner.json* file in the root folder of the integration test project.

    {
      "parallelizeAssembly": false,
      "parallelizeTestCollections": false
    }

### Add XunitHelpers.cs

Create *XunitHelpers.cs* file with the following content in the root folder of the integration test project:

    public class TheoryAttribute : Xunit.TheoryAttribute
    {
        public TheoryAttribute([CallerMemberName] string? memberName = null)
        {
            DisplayName = memberName;
        }
    }

    public class FactAttribute : Xunit.FactAttribute
    {
        public FactAttribute([CallerMemberName] string? memberName = null)
        {
            DisplayName = memberName;
        }
    }

    [Xunit.CollectionDefinition("Integration Tests")]
    public class IntegrationTestCollection : Xunit.ICollectionFixture<TestServerManager>
    {
    }

The first two attributes makes your tests look nicer in the Test Explorer window, the last class is needed for xUnit to register the TestServerManager as a collection fixture.

### Final steps

- Create your first test server with the [DefaultTestServer] attribute as shown in the first section.
- Write your integration tests. Don't forget to inherit from LTestBase and generate the constructor.

Example: 

    public class UserControllerTests : LTestBase
    {
        private readonly HttpRequestBuilder<UserController> _userController;

        public UserControllerTests(TestServerManager serverManager, ITestOutputHelper output)
            : base(serverManager, output)
        {
            _userController = LTestServices.GetHttpRequestBuilder<UserController>();
        }

        [Fact]
        public async Task WhenUserIsLoggedIn_ThenItsDataReturned()
        {
            var token = await _userController.LoginAsAdminAndGetTokenAsync(); // We found it useful to create extension methods for reusing test methods.

            var userData = await _userController
                .CreateFor(x => x.GetUserDataAsync)
                .SetHeaders(x => x.Authorization = new AuthenticationHeaderValue(token))
                .ExecuteSuccessAsync<UserController.UserDto>();

            userData.Username.Should().Be(TestConstants.AdminUsername); // By default FluentAssertions package is available.
        }
    }
