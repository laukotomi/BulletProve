namespace LTest.Hooks
{
    public interface ICleanUpHook : IHook
    {
        Task CleanUpAsync();
    }
}
