using BulletProve.EfCore.Hooks;
using BulletProve.EfCore.Services;
using BulletProve.Logging;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace BulletProve.EfCore.Tests.Hooks
{
    /// <summary>
    /// The clean database hook tests.
    /// </summary>
    public class CleanDatabaseHook_Tests
    {
        private readonly IDatabaseCleanupService _cleanupService;
        private readonly CleanDatabaseHook<Context> _sut;
        private readonly Context _context;
        private readonly ITestLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanDatabaseHook_Tests"/> class.
        /// </summary>
        public CleanDatabaseHook_Tests()
        {
            _context = new Context();
            _logger = Substitute.For<ITestLogger>();
            _cleanupService = Substitute.For<IDatabaseCleanupService>();
            _sut = new CleanDatabaseHook<Context>(_context, _logger, _cleanupService);
        }

        /// <summary>
        /// Tests the before test async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestBeforeTestAsync()
        {
            await _sut.BeforeTestAsync();
            await _cleanupService.Received(1).CleanupAsync(_context);
            _logger.Received(1).LogInformation(Arg.Any<string>());
        }

        /// <summary>
        /// The context.
        /// </summary>
        private class Context : DbContext
        {
        }
    }
}
