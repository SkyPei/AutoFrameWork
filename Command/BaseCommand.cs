using System.Collections.Generic;

namespace ApiFrameWork.Command
{
    public abstract class BaseCommand
    {
        internal List<System.Type> List
        {
            get;set;
        }

       internal string Args
       {
           get;set;
       }

        internal string DataFile
        {
            get;set;
        }
         internal string SheetName
        {
            get;set;
        }
       
       public abstract void Help();
       public  abstract void Run();
       public abstract  void  Run(string args);
    }
}