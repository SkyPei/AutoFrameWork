using System;

namespace ApiFrameWork.Command
{
    public class ClsCommand : BaseCommand
    {
        public override void Help()
        {
            Console.WriteLine("Clear terminal screen.");
        }

        public override void Run()
        {
           Console.Clear();
        }

        public override void Run(string args)
        {
            throw new System.NotImplementedException();
        }
    }
}