# LTest

LTest is an extension to the Microsoft's ASP.NET Core integration test package with useful features.

## Features

### Better management of test servers

By default if you follow the instructions on Microsoft's Integration tests in ASP.NET Core website and use the xUnit's IClassFixture interface for every test class then xUnit will create a new test server for every test class you defines. Initialization of the servers is one of the longest part of running integration tests and doing that many times might not be the best solution.

This framework uses ICollectionFixture to initialize the test server only once and run all of your tests on that instance which can save up a lot of time.

### Easily define test servers

You can define many test servers for different purposes. For example if you need a server with special mocks.

However instead of using a new server it might be better to write configurable mocks and reuse the existing default server.

### Overwrite configuration for testing

Just add integrationtestsettings.json to the root of the integration test project, enable it to be copied to the output directory in the properties panel and you can overwrite the default configuration of your project.

You can also specify your own configuration files using the framework configuration.

### Hooks

You can define integration test lifecycle hooks by implementing lightweight interfaces and registering the class into the DI container.

Hook interfaces:
- IBeforeTestHook: runs before every test.
- IAfterTestHook: runs after every test.
- IAfterServerStarted: runs only once, when the test server was started.
- IResetSingletonHook: a special hook to reset singletons. It will be invoked only for the second and after the seconds tests before running the test logic.
- IBeforeHttpRequestHook (for LTest.Http): runs before every http requests.
- IAfterHttpRequestHook (for LTest.Http): runs after every http requests.

### Logging

While the test is running you have the ability to log useful information in the Test Explorer's output using a wrapper about xUnit's ITestOutputHelper. Also the server logs can be added to the output so you can see exactly what happened. 

### LogSniffer

LogSniffer is a feature that can read the application logs and decide if that log was intented or not. The idea of this feature come up when an application that used Entity Framework logged many warning messages, because the queries were not correctly formatted. By default when LogSniffer is enabled only log levels of information and below are accepted. The rule is that you have to add expected logs before sending the request to the server (ex. warnings when you test bad logins or validation). This can be done in an easy way.

### Http request builder

In the LTest.Http package there is a http request builder feature with which you can create requests easily. You don't have to bother with urls, because the built-in ASP.NET link generator will generate it using the action name.


## Installation

- Create a new 'xUnit Test Project' in Visual Studio.
- Add LTest or LTest.* nuget packages (LTest.Http includes LTest)


After addding the LTest nuget packages there are some small things to do before writing the first unit test.

### Add xunit.runner.json

In order to prevent running integration tests in parallel create xunit.runner.json file in the root folder of the integration test project
