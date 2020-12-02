using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApiFrameWork.Log;



namespace ApiFrameWork.Report
{
    public class ReportModel
    {
        public DateTime StartTime
        {
            get; set;
        }
        public TimeSpan Duration
        {
            get; set;
        }

        public List<ReportItem> TestCases
        {
            get; set;
        }

        public void GenReport(string reportname, string reportfolder)
        {
            Validate(reportfolder);
            string detailfoldername = System.IO.Path.GetFileNameWithoutExtension(reportname);
            // Console.WriteLine(detailfoldername);
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream("ApiFrameWork.Report.report.html");
            var detailresourceStream = assembly.GetManifestResourceStream("ApiFrameWork.Report.log.html");
            string content = string.Empty;
            string detailcontent = string.Empty;

            using (var reader = new System.IO.StreamReader(resourceStream, System.Text.Encoding.UTF8))
            {
                content = reader.ReadToEnd();

            }
            using (var reader = new System.IO.StreamReader(detailresourceStream, System.Text.Encoding.UTF8))
            {
                detailcontent = reader.ReadToEnd();

            }

            int total = 0;
            int pass = 0;
            int fail = 0;
            int error = 0;
            string replaceContent = string.Empty;
            int rownum = 0;
            int key = 0;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Dictionary<string, AFWDictionary<string>> mapping = new Dictionary<string, AFWDictionary<string>>();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Dictionary<string, int> filestores = new Dictionary<string, int>(); //文件存储信息表 ，文件名：存储次数
            foreach (var item in TestCases)
            {
                rownum++;
                total += item.Count;
                pass += item.Pass;
                fail += item.Fail;
                error += item.Error;

                sb.AppendLine($"<tr class='dataEntity'>");
                sb.AppendLine($"<td>{item.Name}</td>");
                sb.AppendLine($"<td></td>");
                sb.AppendLine($"<td></td>");
                sb.AppendLine($"<td class='text-center'>{item.Count}</td>");
                sb.AppendLine($"<td class='text-center'>{item.Pass}</td>");
                sb.AppendLine($"<td class='text-center'>{item.Fail}</td>");
                sb.AppendLine($"<td class='text-center'>{item.Error}</td>");
                sb.AppendLine($"<td class='text-center'>");
                sb.AppendLine($"<a href='javascript:void(0)' class='detail'>Show</a>");
                sb.AppendLine($"</td>");
                sb.AppendLine($"</tr>");
                int index = 0;
                foreach (var detail in item.Details)
                {
                    index++;
                    key++;
                    string detailreportname = String.IsNullOrWhiteSpace(detail.Name) ? $"testData{index}" : detail.Name;
                    if (filestores.Keys.Contains(detailreportname))
                    {
                        int nameindex = filestores[detailreportname] + 1;
                         filestores[detailreportname] = nameindex;
                        detailreportname = $"{detailreportname}({nameindex})";
                       
                    }
                    else
                    {
                        filestores.Add(detailreportname, 0);
                    }
                    sb.AppendLine($"<tr data-level={detail.Status} class='hiddenRow'>");
                    if ("Pass" == detail.Status)
                    {
                        sb.AppendLine($"<td class='passCase'>");
                    }
                    else
                    {
                        sb.AppendLine($"<td class='failCase'>");
                    }
                    sb.AppendLine($"<div class='testcase' data-toggle='modal' data-target='#dataModal' data-whatever='key{key}'>{detailreportname}</div>");
                    mapping.Add($"key{key}", detail.Data);

                    sb.AppendLine($"</td>");
                    sb.AppendLine($"<td align='center'>{detail.Country}</td>");
                    sb.AppendLine($"<td align='center'>{detail.CaseId}</td>");
                    sb.AppendLine($"<td colspan='4' align='center'>");
                    if ("Pass" == detail.Status)
                    {
                        sb.AppendLine($"<span class='label label-success success'>pass</span>");
                    }
                    else
                    {
                        sb.AppendLine($"<span  class='btn btn-danger btn-xs' data-toggle='collapse' data-target='#div_ft{rownum}_{index}'>{detail.Status.ToLower()}</span>");
                        sb.AppendLine($"<div id='div_ft{rownum}_{index}' class='collapse in'>");
                        sb.AppendLine($"<pre>");
                        string errormessage = detail.ErrorMessage;
                        errormessage = errormessage.Replace("<", "&lt;");
                        errormessage = errormessage.Replace(">", "&gt;");
                        sb.AppendLine(errormessage);
                        sb.AppendLine($"</pre>");
                        sb.AppendLine($"</div>");

                    }
                    sb.AppendLine($"</td>");
                    sb.AppendLine($"<td align='center'>");
                    string logMessage = string.Empty;
                    System.Text.StringBuilder logsb = new System.Text.StringBuilder();
                    if (detail.Log != null)
                    {
                        foreach (var log in detail.Log)
                        {
                            if (log.LogType == LogType.StringContent)
                            {
                                string logcontent = log.Content?.Replace("<", "&lt;").Replace(">", "&gt;");
                                if (log.Style == Style.Default)
                                {
                                    logsb.Append(logcontent);
                                }
                                else
                                {
                                    string color = log.Style == Style.Yellow ? "background-color:#ead158!important;float:left" : "background-color: #6da4ea!important;float:left;";

                                    logsb.Append($"<div style=\"{color}\">{logcontent}</div>");
                                }
                            }
                            else
                            {
                                string imagePath = string.Empty;
                                string base64url = string.Empty;
                                if (reportfolder != null)
                                {
                                    imagePath = $"{reportfolder}\\Pic\\{log.Content}";
                                }
                                else
                                {
                                    imagePath = $"Pic\\{log.Content}";
                                }
                                using (System.IO.FileStream fs = new System.IO.FileStream(imagePath, System.IO.FileMode.Open))
                                {
                                    byte[] byteData = new byte[fs.Length];
                                    fs.Read(byteData, 0, byteData.Length);
                                    base64url = Convert.ToBase64String(byteData);
                                }


                                logsb.AppendLine($"<img src='data:image/png;base64,{base64url}' alt='' style='max-width: 100%;' />");


                            }
                        }
                    }

                    logMessage = logsb.ToString();
                    if (string.IsNullOrEmpty(logMessage))
                    {
                        logMessage = "&lt;None&gt;";
                    }
                    string newdetailcontent = string.Empty;

                    newdetailcontent = detailcontent.Replace("#{Content}", logMessage);
                    if (string.IsNullOrEmpty(reportfolder))
                    {

                        if (!System.IO.Directory.Exists(detailfoldername))
                        {
                            System.IO.Directory.CreateDirectory(detailfoldername);
                        }


                    }
                    else
                    {
                        if (!System.IO.Directory.Exists(reportfolder))
                        {
                            System.IO.Directory.CreateDirectory(reportfolder);
                        }

                        if (!System.IO.Directory.Exists($"{reportfolder}\\{detailfoldername}"))
                        {
                            System.IO.Directory.CreateDirectory($"{reportfolder}\\{detailfoldername}");
                        }


                    }
                    string detailreport = string.IsNullOrEmpty(reportfolder) ? $"{detailfoldername}\\{detailreportname}_{detail.Status.ToLower()}.html" : $"{reportfolder}\\{detailfoldername}\\{detailreportname}_{detail.Status.ToLower()}.html";
                    // Console.WriteLine(detailreport);
                    using (var writer = System.IO.File.Create(detailreport))
                    {
                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(newdetailcontent);
                        writer.Write(bytes, 0, bytes.Length);
                    }


                    sb.AppendLine($"<a href='{detailfoldername}\\{detailreportname}_{detail.Status.ToLower()}.html'  class='detail'>View Log</a>");
                    sb.AppendLine($"</td>");
                    sb.AppendLine($"</tr>");

                }


            }
            decimal rate = Math.Round((decimal)pass / (decimal)total * (decimal)100, 2);
            content = content.Replace("#{mapping}", Newtonsoft.Json.JsonConvert.SerializeObject(mapping));
            // content = content.Replace("#{data}", Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            content = content.Replace("#{Content}", sb.ToString());
            content = content.Replace("#{Start Time}", StartTime.ToString());
            content = content.Replace("#{Duration}", Duration.ToString());
            content = content.Replace("#{Status}", $"All:{total} Pass:{pass} Failure:{fail} Error:{error}, Passing rate:{rate}%");
            content = content.Replace("#{Summary}", $"{rate}%");
            content = content.Replace("#{Failed}", fail.ToString());
            content = content.Replace("#{Passed}", pass.ToString());
            content = content.Replace("#{All}", total.ToString());
            content = content.Replace("#{Error}", error.ToString());



            string report = string.IsNullOrEmpty(reportfolder) ? reportname : $"{reportfolder}\\{reportname}";
            using (var writer = System.IO.File.Create(report))
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
                writer.Write(bytes, 0, bytes.Length);
            }

        }


        public void GenBatchReport(string reportname, string reportfolder)
        {
            Validate(reportfolder);
            string detailfoldername = System.IO.Path.GetFileNameWithoutExtension(reportname);
            // Console.WriteLine(detailfoldername);
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream("ApiFrameWork.Report.report.html");
            var detailresourceStream = assembly.GetManifestResourceStream("ApiFrameWork.Report.log.html");
            string content = string.Empty;
            string detailcontent = string.Empty;

            using (var reader = new System.IO.StreamReader(resourceStream, System.Text.Encoding.UTF8))
            {
                content = reader.ReadToEnd();

            }
            using (var reader = new System.IO.StreamReader(detailresourceStream, System.Text.Encoding.UTF8))
            {
                detailcontent = reader.ReadToEnd();

            }

            int total = 0;
            int pass = 0;
            int fail = 0;
            int error = 0;
            string replaceContent = string.Empty;
            int rownum = 0;
            int key = 0;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Dictionary<string, AFWDictionary<string>> mapping = new Dictionary<string, AFWDictionary<string>>();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var item in TestCases)
            {
                rownum++;
                total += item.Count;
                pass += item.Pass;
                fail += item.Fail;
                error += item.Error;

                sb.AppendLine($"<tr class='dataEntity'>");
                sb.AppendLine($"<td>{item.Name}</td>");
                sb.AppendLine($"<td></td>");
                sb.AppendLine($"<td></td>");
                sb.AppendLine($"<td class='text-center'>{item.Count}</td>");
                sb.AppendLine($"<td class='text-center'>{item.Pass}</td>");
                sb.AppendLine($"<td class='text-center'>{item.Fail}</td>");
                sb.AppendLine($"<td class='text-center'>{item.Error}</td>");
                sb.AppendLine($"<td class='text-center'>");
                sb.AppendLine($"<a href='javascript:void(0)' class='detail'>Show</a>");
                sb.AppendLine($"</td>");
                sb.AppendLine($"</tr>");
                int index = 0;
                if (item.Details.Count > 0)
                {
                    var lastlog = item.Details.LastOrDefault()?.Log;
                    if (lastlog != null)
                    {
                        int dividentifyflag = -1;
                        System.Text.StringBuilder logsb = new System.Text.StringBuilder();
                        foreach (var log in lastlog)
                        {
                            if (log.LogType == LogType.StringContent)
                            {
                                if (log.Section != null)
                                {
                                    if (dividentifyflag > -1)
                                    {
                                        logsb.AppendLine($"</div>");
                                    }
                                    logsb.AppendLine($"<div id=\"d{log.Section.Value}\">");
                                    dividentifyflag = log.Section.Value;
                                    continue;
                                }
                                string logcontent = log.Content?.Replace("<", "&lt;").Replace(">", "&gt;");
                                if (log.Style == Style.Default)
                                {
                                    logsb.Append(logcontent);
                                }
                                else
                                {
                                    string color = log.Style == Style.Yellow ? "background-color:#ead158!important;float:left" : "background-color: #6da4ea!important;float:left;";

                                    logsb.Append($"<div style=\"{color}\">{logcontent}</div>");
                                }
                            }
                            else
                            {
                                string imagePath = string.Empty;
                                string base64url = string.Empty;
                                if (reportfolder != null)
                                {
                                    imagePath = $"{reportfolder}\\Pic\\{log.Content}";
                                }
                                else
                                {
                                    imagePath = $"Pic\\{log.Content}";
                                }
                                using (System.IO.FileStream fs = new System.IO.FileStream(imagePath, System.IO.FileMode.Open))
                                {
                                    byte[] byteData = new byte[fs.Length];
                                    fs.Read(byteData, 0, byteData.Length);
                                    base64url = Convert.ToBase64String(byteData);
                                }


                                logsb.AppendLine($"<img src='data:image/png;base64,{base64url}' alt='' style='max-width: 100%;' />");


                            }
                        }
                        if (dividentifyflag != -1)
                        {
                            logsb.AppendLine($"</div>");
                        }





                        string logMessage = logsb.ToString();
                        if (string.IsNullOrEmpty(logMessage))
                        {
                            logMessage = "&lt;None&gt;";
                        }
                        string newdetailcontent = string.Empty;

                        newdetailcontent = detailcontent.Replace("#{Content}", logMessage);
                        if (string.IsNullOrEmpty(reportfolder))
                        {

                            if (!System.IO.Directory.Exists(detailfoldername))
                            {
                                System.IO.Directory.CreateDirectory(detailfoldername);
                            }


                        }
                        else
                        {
                            if (!System.IO.Directory.Exists(reportfolder))
                            {
                                System.IO.Directory.CreateDirectory(reportfolder);
                            }

                            if (!System.IO.Directory.Exists($"{reportfolder}\\{detailfoldername}"))
                            {
                                System.IO.Directory.CreateDirectory($"{reportfolder}\\{detailfoldername}");
                            }


                        }
                        string detailreport = string.IsNullOrEmpty(reportfolder) ? $"{detailfoldername}\\{item.Name}_testData.html" : $"{reportfolder}\\{detailfoldername}\\{item.Name}_testData.html";
                        // Console.WriteLine(detailreport);
                        using (var writer = System.IO.File.Create(detailreport))
                        {
                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(newdetailcontent);
                            writer.Write(bytes, 0, bytes.Length);
                        }


                    }

                }

                foreach (var detail in item.Details)
                {

                    key++;
                    sb.AppendLine($"<tr data-level={detail.Status} class='hiddenRow'>");
                    if ("Pass" == detail.Status)
                    {
                        sb.AppendLine($"<td class='passCase'>");
                    }
                    else
                    {
                        sb.AppendLine($"<td class='failCase'>");
                    }
                    sb.AppendLine($"<div class='testcase' data-toggle='modal' data-target='#dataModal' data-whatever='key{key}'>testData{index}</div>");
                    mapping.Add($"key{key}", detail.Data);

                    sb.AppendLine($"</td>");
                    sb.AppendLine($"<td align='center'>{detail.Country}</td>");
                    sb.AppendLine($"<td align='center'>{detail.CaseId}</td>");
                    sb.AppendLine($"<td colspan='4' align='center'>");
                    if ("Pass" == detail.Status)
                    {
                        sb.AppendLine($"<span class='label label-success success'>pass</span>");
                    }
                    else
                    {
                        sb.AppendLine($"<span  class='btn btn-danger btn-xs' data-toggle='collapse' data-target='#div_ft{rownum}_{index}'>{detail.Status.ToLower()}</span>");
                        sb.AppendLine($"<div id='div_ft{rownum}_{index}' class='collapse in'>");
                        sb.AppendLine($"<pre>");
                        string errormessage = detail.ErrorMessage;
                        errormessage = errormessage.Replace("<", "&lt;");
                        errormessage = errormessage.Replace(">", "&gt;");
                        sb.AppendLine(errormessage);
                        sb.AppendLine($"</pre>");
                        sb.AppendLine($"</div>");

                    }
                    sb.AppendLine($"</td>");
                    sb.AppendLine($"<td align='center'>");



                    sb.AppendLine($"<a href='{detailfoldername}\\{item.Name}_testData.html#d{index}'  class='detail'>View Log</a>");
                    sb.AppendLine($"</td>");
                    sb.AppendLine($"</tr>");
                    index++;
                }


            }
            decimal rate = Math.Round((decimal)pass / (decimal)total * (decimal)100, 2);
            content = content.Replace("#{mapping}", Newtonsoft.Json.JsonConvert.SerializeObject(mapping));
            // content = content.Replace("#{data}", Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            content = content.Replace("#{Content}", sb.ToString());
            content = content.Replace("#{Start Time}", StartTime.ToString());
            content = content.Replace("#{Duration}", Duration.ToString());
            content = content.Replace("#{Status}", $"All:{total} Pass:{pass} Failure:{fail} Error:{error}, Passing rate:{rate}%");
            content = content.Replace("#{Summary}", $"{rate}%");
            content = content.Replace("#{Failed}", fail.ToString());
            content = content.Replace("#{Passed}", pass.ToString());
            content = content.Replace("#{All}", total.ToString());
            content = content.Replace("#{Error}", error.ToString());



            string report = string.IsNullOrEmpty(reportfolder) ? reportname : $"{reportfolder}\\{reportname}";
            using (var writer = System.IO.File.Create(report))
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
                writer.Write(bytes, 0, bytes.Length);
            }

        }



        private void Validate(string reportfolder)
        {

            if (!string.IsNullOrEmpty(reportfolder) && !System.IO.Directory.Exists(reportfolder))
            {
                System.IO.Directory.CreateDirectory(reportfolder);
            }
            if (string.IsNullOrEmpty(reportfolder))
            {

                if (!System.IO.Directory.Exists("Pic"))
                {
                    System.IO.Directory.CreateDirectory("Pic");
                }


            }
            else
            {

                if (!System.IO.Directory.Exists($"{reportfolder}\\Pic"))
                {
                    System.IO.Directory.CreateDirectory($"{reportfolder}\\Pic");
                }


            }



        }

        private void WriteResourceFile(string folderfile, string resourcefile)
        {
            if (!System.IO.File.Exists(folderfile))
            {
                var assembly = Assembly.GetExecutingAssembly();

                var resourceStream = assembly.GetManifestResourceStream(resourcefile);
                using (var writer = System.IO.File.Create(folderfile))
                {
                    using (var reader = new System.IO.StreamReader(resourceStream, System.Text.Encoding.UTF8))
                    {
                        string content = reader.ReadToEnd();
                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
                        writer.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }


    }

    public class ReportItem
    {
        public int Count
        {
            get; set;
        } = 0;

        public int Pass
        {
            get; set;
        } = 0;
        public int Fail
        {
            get; set;
        } = 0;

        public int Error
        {
            get; set;
        } = 0;

        public string Name
        {
            get; set;
        }


        public List<ReportItemDetail> Details
        {
            get; set;
        }
    }

    public class ReportItemDetail
    {
        public string Name
        {
            get; set;
        }
        public AFWDictionary<string> Data
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }

        public List<LogStore> Log
        {
            get; set;
        }

        public string ErrorMessage
        {
            get; set;
        }

        public string Country
        {
            get; set;
        }

        public string CaseId
        {
            get; set;
        }


    }
}