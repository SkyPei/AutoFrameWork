using System;
using System.Collections.Generic;
using System.Reflection;
using ApiFrameWork.Schema;
using System.Linq;
using System.Data;
using ApiFrameWork.Utility;
using ApiFrameWork.Report;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;


namespace ApiFrameWork.Command
{
    public class BatchrunCommand : BaseCommand
    {
        public override void Help()
        {
            Console.WriteLine("Runs a script in a batch way.");
            Console.WriteLine("");
            Console.WriteLine("batchrun [name]");
            Console.WriteLine("");
            Console.WriteLine("    [name]");
            Console.WriteLine("          Specifies script name");

        }

        public override void Run()
        {
            throw new System.NotImplementedException();
        }

        public override void Run(string args)
        {
            Dictionary<string, Type> names = new Dictionary<string, Type>();
            string data = args;
            foreach (var item in List)
            {
                var attribute = item.GetCustomAttribute<ScriptAttribute>();
                string name = attribute == null ? item.Name.ToLower() : attribute.DisplayName.ToLower();
                names.Add(name, item);
            }

            var type = names.FirstOrDefault(t => t.Key == data.ToLower()).Value;

            if (type == null)
            {
                throw new Exception.CommandPaseException($"Not Found Script {{{data}}}");
            }
            var scriptinfo = type.GetCustomAttribute<ScriptAttribute>();


            string file = scriptinfo.DataFile;


            string sheet = scriptinfo.SheetName;



            DataProvider dp = new DataProvider(file, sheet);
            ProviderModel dt = dp.ExtractData(data);
            var list = dt.list.Where(t => t.IsEnabled).ToList();
            ReportModel rm = new ReportModel();
            rm.StartTime = DateTime.Now;
            rm.TestCases = new List<ReportItem>();
            ReportItem testcase = new ReportItem();
            testcase.Name = data;
            testcase.Count = list.Count;
            testcase.Details = new List<ReportItemDetail>();
            rm.TestCases.Add(testcase);
            LaunchConfig config = LaunchConfig.GetInstance();


            int index = 0;

            // System.Threading.Thread.Sleep(1000);

            string currentdate = DateTime.Now.ToString("MMddyyyyHHmmss");
            //启动日志
            Script script = null;
            using (Log.Log log = Log.Log.GetLog($"{data}_{currentdate}_{index}", config.LogFolderPath))
            {
                using (Log.ResponseContent contents = Log.ResponseContent.GetInstance())
                {
                    foreach (DataModel row in list)
                    {
                        ReportItemDetail rd = new ReportItemDetail();
                        testcase.Details.Add(rd);

                        try
                        {
                            if (script == null)
                            {

                                script = Activator.CreateInstance(type) as Script;
                                script.Log = log;
                               
                            }
                            log.SetSection(index);
                            script.Test(row.Dict);
                            testcase.Pass += 1;
                            rd.Status = "Pass";
                        }
                        catch (Exception.AssertionException e)
                        {
                            Console.WriteLine(e.Message);
                            testcase.Fail += 1;
                            rd.Status = "Fail";
                            rd.ErrorMessage = e.Message;
                            if (string.IsNullOrEmpty(script.Country))
                            {
                                var countrydata = row.Dict.FirstOrDefault(t => "country" == t.Key.ToLower());
                                if (!string.IsNullOrEmpty(countrydata.Value))
                                {
                                    rd.Country = countrydata.Value;
                                }
                            }
                            else
                            {
                                rd.Country = script.Country;

                            }

                            if (string.IsNullOrEmpty(script.CaseId))
                            {
                                var caseiddata = row.Dict.FirstOrDefault(t => "caseid" == t.Key.ToLower());
                                if (!string.IsNullOrEmpty(caseiddata.Value))
                                {
                                    rd.CaseId = caseiddata.Value;
                                }
                            }
                            else
                            {
                                rd.CaseId = script.CaseId;

                            }



                            log.Info("-------------------------Failed------------------------\r\n");
                            log.Info($"{e.Message}\r\n");
                            log.Info("-------------------------Tracking---------------------\r\n");
                            log.Info($"{e.StackTrace}\r\n");



                        }
                        catch (System.Exception e)
                        {

                            var innerex = GetInnerException(e);
                            log.Info("-------------------------Error------------------------\r\n");
                            log.Info($"{innerex.Message}\r\n");
                            log.Info("-------------------------Tracking---------------------\r\n");
                            log.Info($"{innerex.StackTrace}\r\n");
                            Console.WriteLine(innerex.Message);
                            testcase.Error += 1;
                            rd.Status = "Error";
                            rd.ErrorMessage = e.Message;

                            if (string.IsNullOrEmpty(script.Country))
                            {
                                var countrydata = row.Dict.FirstOrDefault(t => "country" == t.Key.ToLower());
                                if (!string.IsNullOrEmpty(countrydata.Value))
                                {
                                    rd.Country = countrydata.Value;
                                }
                            }
                            else
                            {
                                rd.Country = script.Country;

                            }

                            if (string.IsNullOrEmpty(script.CaseId))
                            {
                                var caseiddata = row.Dict.FirstOrDefault(t => "caseid" == t.Key.ToLower());
                                if (!string.IsNullOrEmpty(caseiddata.Value))
                                {
                                    rd.CaseId = caseiddata.Value;
                                }
                            }
                            else
                            {
                                rd.CaseId = script.CaseId;

                            }

                           
                            var dparameters = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(t => t.GetCustomAttribute<InjectAttribute>() != null).ToList();
                            foreach (var parameter in dparameters)
                            {
                                var objvalue = parameter.GetValue(script);
                                if (objvalue != null)
                                {
                                    var returntype = typeof(RegistrationBuilder<>).MakeGenericType(parameter.FieldType);
                                    var parobj = Convert.ChangeType(config.GetRB(parameter.FieldType), returntype);
                                    if (parobj != null)
                                    {
                                        MethodInfo handlerMethod = returntype.GetProperty("EndHandle").GetValue(parobj).GetType().GetMethod("Invoke");
                                        handlerMethod.Invoke(returntype.GetProperty("EndHandle").GetValue(parobj), new object[] { objvalue, log });
                                    }
                                }
                            }
                            
                            script = null;
                        }





                        if (string.IsNullOrEmpty(script.Country))
                        {
                            var countrydata = row.Dict.FirstOrDefault(t => "country" == t.Key.ToLower());
                            if (!string.IsNullOrEmpty(countrydata.Value))
                            {
                                rd.Country = countrydata.Value;
                            }
                        }
                        else
                        {
                            rd.Country = script.Country;

                        }

                        if (string.IsNullOrEmpty(script.CaseId))
                        {
                            var caseiddata = row.Dict.FirstOrDefault(t => "caseid" == t.Key.ToLower());
                            if (!string.IsNullOrEmpty(caseiddata.Value))
                            {
                                rd.CaseId = caseiddata.Value;
                            }
                        }
                        else
                        {
                            rd.CaseId = script.CaseId;

                        }


                        
                        rd.Data = row.Dict;
                        index++;
                    }
                    var lastlog= new List<Log.LogStore>();
                    foreach (var itemstore in log.Stores)
                    {
                        lastlog.Add(new Log.LogStore{Content=itemstore.Content, Style=itemstore.Style,LogType=itemstore.LogType,Section=itemstore.Section});
                    }
                    testcase.Details.Last().Log=lastlog;
                    if (script != null)
                    {

                        var parameters = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(t => t.GetCustomAttribute<InjectAttribute>() != null).ToList();
                        foreach (var parameter in parameters)
                        {
                            var objvalue = parameter.GetValue(script);
                            if (objvalue != null)
                            {
                                var returntype = typeof(RegistrationBuilder<>).MakeGenericType(parameter.FieldType);
                                var parobj = Convert.ChangeType(config.GetRB(parameter.FieldType), returntype);
                                if (parobj != null)
                                {
                                    MethodInfo handlerMethod = returntype.GetProperty("EndHandle").GetValue(parobj).GetType().GetMethod("Invoke");
                                    handlerMethod.Invoke(returntype.GetProperty("EndHandle").GetValue(parobj), new object[] { objvalue, log });
                                }
                            }
                        }
                    }
                }
            }



            try
            {
                dp.Update(dt);
            }
            catch (System.Exception ee)
            {
                Console.WriteLine(ee.Message);
            }

            rm.Duration = (DateTime.Now - rm.StartTime).Duration();
            rm.GenBatchReport($"{data}_{DateTime.Now.ToString("MMddyyyyHHmmss")}.html", config.LogFolderPath);
            Console.WriteLine("Script completed execution.");
        }


        private System.Exception GetInnerException(System.Exception ex)
        {
            if (ex.InnerException == null)
                return ex;
            else
                return GetInnerException(ex.InnerException);
        }
    }
}