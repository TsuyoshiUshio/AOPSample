using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOPLib
{
    [Function(isLogging: true)]
    public class SampleFunction : IFunction
    {
        public void Execute(FunctionContext context)
        {
            Console.WriteLine($"Sample Function Executed! Name:{context.Name} TraceId: {context.TraceId} ");
        }
    }
}
