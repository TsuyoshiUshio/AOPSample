using AOPLib;

namespace AOPSample;

partial class Program
{
    public static void Main(string[] args)
    {
        var functionName = Console.ReadLine();
        IFunction function = AOPLib.FunctionFactory.CreateFunction(functionName);
        function.Execute(new FunctionContext() { Name = "hello", TraceId = Guid.NewGuid() });
    }
}