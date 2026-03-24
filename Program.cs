
using AutoFrameWork;
using AutoFrameWork.Schema;
using AutoFrameWork.Utility;

namespace AutoFrameWork
{
    class Program
    {
        static void Main(string[] args)
        {
         
           
        }



    }

//     [Script(DisplayName = "test", App = "Testing", DataFile = "testdata.xlsx")]
// public class MyTest : AutoFrameWork.Script
// {
//     public override void Test(AFWDictionary<string> row)
//     {
//             System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create($"http://10.63.144.191:10034/CLMP");
//         request.Method = "POST";
//         request.ContentType = "application/json";

//         var data = new {
//           Head=new {BsnSeqNo= "B000HOPA202512150000021661", CnsmrSeqNo=$"43069522262556{new System.Random().Next(1, 10000).ToString("D4")}", CnsmrSysId="501200", FileFlg= "0", InstId= "50701", SrcCnsmrSeqNo= $"10972917121277{new System.Random().Next(1, 10000).ToString("D4")}", SrcCnsmrSysId= "501200", SvcCd="30230001", SvcScn="26", TlrNo= "V0055", TxnCd= "8101", TxnDt= "20260104", TxnTm= "175426", machineRoom= "A"},
//           Body=new {DblNo= "098222101356769787002906"}
//         };


//             Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
//         settings.Formatting = Newtonsoft.Json.Formatting.Indented;

//         var datastring = Newtonsoft.Json.JsonConvert.SerializeObject(data,settings);

//       var response=  request.Send(datastring);

//       var responsedata =response.GetResponseBody();
        
       
//     }
// }




}
