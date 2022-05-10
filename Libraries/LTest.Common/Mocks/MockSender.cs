using LTest.Interfaces;
using System.Collections.Generic;

namespace LTest.Mocks
{
    /// <summary>
    /// Mock sender.
    /// </summary>
    /// <typeparam name="T">Model to store.</typeparam>
    public abstract class MockSender<T> : ICleanSingletonBeforeTest
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
        /// Clear sent messages;
        /// </summary>
        public void Clear()
        {
            Messages.Clear();
        }
    }
}