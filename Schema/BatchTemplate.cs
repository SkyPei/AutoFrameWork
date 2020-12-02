using System.Collections.Generic;

namespace ApiFrameWork.Schema
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

    }
}