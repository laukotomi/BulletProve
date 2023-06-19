using LTest.Hooks;

namespace LTest.Mocks
{
    /// <summary>
    /// Mock sender.
    /// </summary>
    /// <typeparam name="T">Model to store.</typeparam>
    public abstract class LTestMockSender<T> : ICleanUpHook
    {
        /// <summary>
        /// Sent messages;
        /// </summary>
        protected readonly List<T> Messages = new();

        /// <summary>
        /// Access sent messages.
        /// </summary>
        public IReadOnlyList<T> SentMessages => Messages;

        public Task CleanUpAsync()
        {
            Messages.Clear();
            return Task.CompletedTask;
        }
    }
}