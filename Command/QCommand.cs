using System;
using System.Collections.Generic;

namespace ApiFrameWork.Command
{
    public class QCommand : BaseCommand
    {

        public override void Help()
        {
             Console.WriteLine("Quits the command interpreter.");
        }
        public override void Run()
        {

            Environment.Exit(0);
        }

        public override void Run(string args)
        {
            throw new NotImplementedException();
        }
    }
}