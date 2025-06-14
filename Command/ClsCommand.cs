using System;

namespace AutoFrameWork.Command
{
    public class ClsCommand : BaseCommand
    {
        public override void Help()
        {
            Console.WriteLine("Clear terminal screen.");
        }

        public override bool Run()
        {
           Console.Clear();
            return true;
        }

        public override bool Run(string args)
        {
            throw new System.NotImplementedException();
        }
    }
}