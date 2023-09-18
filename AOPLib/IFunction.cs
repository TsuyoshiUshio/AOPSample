using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOPLib
{
    public interface IFunction
    {
        public void Execute(FunctionContext context);
    }
}
