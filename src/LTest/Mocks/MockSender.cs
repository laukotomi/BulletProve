using LTest.Hooks;

namespace LTest.Mocks
{
    /// <summary>
    /// Mock sender.
    /// </summary>
    /// <typeparam name="T">Model to store.</typeparam>
    public abstract class MockSender<T> : IResetSingletonHook
    {
        /// <summary>
        /// Sent messages;
        /// </summary>
        protected readonly List<T> Messages = new();

        /// <summary>
        /// Access sent messages.
        /// </summary>
        public IReadOnlyList<T> SentMessages => Messages;

        /// <summary>
        /// Resets the class.
        /// </summary>
        /// <returns>A Task.</returns>
        public Task ResetAsync()
        {
            Messages.Clear();
            return Task.CompletedTask;
        }
    }
}