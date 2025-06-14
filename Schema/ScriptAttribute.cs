using System;
using System.Runtime.Serialization;

namespace AutoFrameWork.Schema
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
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
            get; set;
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
            get; set;
        }
        public string SheetName
        {
            get; set;
        }

        public string[] TestCaseId
        {
            get; set;
        }

        public Type DataSource
        {
            get; set; 
        }
    }


    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DataSourceInAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DataSourceOutAttribute : Attribute
    {

    }

    public abstract class DataSource
    {
        public abstract System.Collections.IList ReadDataSource(string scriptname, AFWDictionary<string> row);
        public virtual void WriteDataSource(string scriptname, AFWDictionary<string> row, object valuie, ScriptStatus status)
        {

        }
    }

    public enum ScriptStatus
    {
        Pass,
        Fail,
        Error
    }

    public enum UploadStatus
    {
        [EnumMember(Value ="Not Enabled")]
        NotEnabled,
        Pass,
        Error
    }
}