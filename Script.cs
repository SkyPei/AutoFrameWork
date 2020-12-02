namespace ApiFrameWork
{
    public abstract class Script :IScript
    {

        public ApiFrameWork.Log.Log Log
        {
                get;
                internal set;
        }
        
        public string Country
        {
            get;set;
        }

        public string CaseId
        {
            get;set;
        }

        public string InstanceId
        {
            get;set;
        }

        public string ReportName
        {
            get;set;
        }

        public abstract void Test(AFWDictionary<string> row);
       
    }
}