using System.Threading.Tasks;

namespace AutoFrameWork
{
    public abstract class Script :IScript
    {

        public AutoFrameWork.Log.Log Log
        {
                get;
                internal set;
        }
        
     

        public string CaseId
        {
            get;set;
        }

      

        public string ReportName
        {
            get;set;
        }

        public string ScriptId
        {
            get;set;
        }

        public dynamic AdditionalInfo
        {
            get; private set;
        } = new Schema.DynamicPropertyView();

        public abstract void Test(AFWDictionary<string> row);
       
    }


    public abstract class ScriptAsync : IScript
    {
        public AutoFrameWork.Log.Log Log
        {
            get;
            internal set;
        }




        public string CaseId
        {
            get; set;
        }



        public string ReportName
        {
            get; set;
        }

        public string ScriptId
        {
            get; set;
        }

        public dynamic AdditionalInfo
        {
            get; private set;
        } = new Schema.DynamicPropertyView();

        public abstract Task TestAsync(AFWDictionary<string> row);

        public void Test(AFWDictionary<string> row)
        {
            TestAsync(row).GetAwaiter().GetResult();
        }

    }
}