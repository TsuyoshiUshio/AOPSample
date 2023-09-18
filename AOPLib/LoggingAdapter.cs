using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOPLib
{
    public class LoggingAdapter : IFunction
    {
        private readonly IFunction _function;
        public LoggingAdapter(IFunction function)
        {
            _function = function;
        }
        public void Execute(FunctionContext context)
        {
            Console.WriteLine($"[{DateTime.UtcNow}] {_function.GetType().Name}: before execute. TraceId: {context.TraceId}");
            _function.Execute(context);
            Console.WriteLine($"[{DateTime.UtcNow}] {_function.GetType().Name}: after execute. TraceId: {context.TraceId}");
        }
    }
}
