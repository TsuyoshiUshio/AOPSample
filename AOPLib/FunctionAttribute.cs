namespace AOPLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FunctionAttribute : Attribute
    {
        public bool IsLogging { get; private set; }
        public FunctionAttribute(bool isLogging = false)
        {
            IsLogging = isLogging;
        }
    }
}