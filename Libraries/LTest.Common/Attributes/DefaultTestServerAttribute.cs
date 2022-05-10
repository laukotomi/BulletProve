using System;

namespace LTest.Attributes
{
    /// <summary>
    /// Default test server.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultTestServerAttribute : Attribute
    {
    }
}
