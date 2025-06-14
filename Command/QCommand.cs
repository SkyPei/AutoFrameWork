using System;
using System.Collections.Generic;

namespace AutoFrameWork.Command
{
    public class QCommand : BaseCommand
    {

        public override void Help()
        {
             Console.WriteLine("Quits the command interpreter.");
        }
        public override bool Run()
        {

            return false;
        }

        public override bool Run(string args)
        {
            throw new NotImplementedException();
        }
    }
}