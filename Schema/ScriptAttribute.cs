using System;

namespace ApiFrameWork.Schema
{
    [AttributeUsage(AttributeTargets.Class ,  AllowMultiple = false)]
     public class ScriptAttribute : Attribute
    {
        public string DisplayName
        {
            get; set;
        }

        public string LOB
        {
            get; set;
        }

        public string App
        {
            get;set;
        }

        public bool Visible
        {
            get; set;
        } = true;

        public string Creator
        {
            get; set;
        }

        public string DataFile
        {
            get;set;
        }
        public string SheetName
        {
            get;set;
        }

        public string[] TestCaseId
        {
            get;set;
        }
    }


    [AttributeUsage(AttributeTargets.Field,  AllowMultiple = false)]
     public class InjectAttribute : Attribute
    {
       
    }
}