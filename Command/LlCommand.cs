using System;
using System.Collections.Generic;
using System.Reflection;
using ApiFrameWork.Schema;

namespace ApiFrameWork.Command
{
    public class LlCommand : BaseCommand
    {
       
        public override void Help()
        {
            Console.WriteLine("Displays a list of scripts in project.");
        }

        public override void Run()
        {
            foreach (var item in List)
            {
                var attribute = item.GetCustomAttribute<ScriptAttribute>();
                string name = attribute == null ? item.Name : attribute.DisplayName;
                Console.WriteLine(name);
            }
             Console.Write("\r\n");
        }

        public override void Run(string args)
        {
            throw new System.NotImplementedException();
        }
    }
}