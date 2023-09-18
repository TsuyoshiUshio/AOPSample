using AOPLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOPSample
{
    [Function(isLogging: false)]
    public class NoLoggingFunction : IFunction
    {
        public void Execute(FunctionContext context)
        {
            Console.WriteLine($"[{nameof(NoLoggingFunction)}]: execute. Name: {context.Name}");
        }
    }
}
