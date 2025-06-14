using System.Collections.Generic;

namespace AutoFrameWork.Command
{
    public abstract class BaseCommand
    {
        internal IResultUpload Upload
        {
            get; set; 
        }
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
       public  abstract bool Run();
       public abstract  bool  Run(string args);
    }
}