using Xunit.Abstractions;

namespace BulletProve
{
    /// <summary>
    /// The output adapter.
    /// </summary>
    internal class OutputAdapter : IOutput
    {
        private readonly ITestOutputHelper _outputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputAdapter"/> class.
        /// </summary>
        /// <param name="outputHelper">The output helper.</param>
        public OutputAdapter(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        /// <inheritdoc/>
        public void WriteLine(string text)
        {
            _outputHelper.WriteLine(text);
        }
    }
}
