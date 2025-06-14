using System.Collections.Generic;

namespace AutoFrameWork.Schema
{
    public class BatchTemplate
    {
        public string File
        {
            get; set;
        } = "";

        public string Sheet
        {
            get; set;
        } = "";

        public string Schedule
        {
            get;set;
        }="";
        public List<BatchGroup> Batch
        {
            get; set;
        }

        public bool IsAsync
        { get; set; } = false;

        public int MaxThreads
        { get; set; } = 10;
    }

    public class BatchGroup
    {
        public List<BatchGroupItem> Groups
        {
            get; set;
        }
        public bool IsDependencyQueue
        {
            get; set;
        }
        public string SetId
            { get; set; }
    }

    public class BatchGroupItem
    {
        public string name
        {
            get; set;
        } = "";
        public string File
        {
            get; set;
        } = "";

        public string Sheet
        {
            get; set;
        } = "";

        private string _scriptid = null;

        public string scriptid
        {
            get => _scriptid;
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    _scriptid = value;
                }
            }
        }
    }
}