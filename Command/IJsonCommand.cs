using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFrameWork.Command
{
    internal interface IJsonCommand
    {
        void Run(object input);
    }
}
