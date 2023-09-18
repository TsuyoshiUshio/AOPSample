using AOPLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOPSample
{
    [Function(isLogging: true)]
    public class LoggingFunction : IFunction
    {
        public void Execute(FunctionContext context)
        {
            Console.WriteLine($"[{nameof(LoggingFunction)}]: execute. Name: {context.Name}");
        }
    }
}
