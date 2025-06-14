using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoFrameWork.Log;
using AutoFrameWork.Schema;
using AutoFrameWork.Utility;
using NPOI.SS.Formula.Functions;



namespace AutoFrameWork.Report
{
    public class ReportModel
    {
        private object obj = new object();
        public Dictionary<string, int> filestores = new Dictionary<string, int>();
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
        public bool IsBatchMode
        { get; private set; }

        private IResultUpload _upload;

        public ReportModel(IResultUpload upload, bool isbatchmode)
        {
            _upload = upload;
            IsBatchMode = isbatchmode;
        }

        /// <summary>
        /// position of water mark 
        /// </summary>
        /// <param name="pos"></param>
        /// 1 top left  , 2 top center , 3 top right
        /// 4 middle left , 5 middle , 6 middle right
        /// 7 bottom left , 8 bottom center , 9 buttom right
        /// <returns></returns>
        private StringFormat GetStringFormat(int pos)
        {
            StringFormat format = new StringFormat();
            switch (pos)
            {
                case 1: format.Alignment = StringAlignment.Near; format.LineAlignment = StringAlignment.Near; break;
                case 2: format.Alignment = StringAlignment.Center; format.LineAlignment = StringAlignment.Near; break;
                case 3: format.Alignment = StringAlignment.Far; format.LineAlignment = StringAlignment.Near; break;
                case 4: format.Alignment = StringAlignment.Near; format.LineAlignment = StringAlignment.Center; break;
                case 5: format.Alignment = StringAlignment.Center; format.LineAlignment = StringAlignment.Center; break;
                case 6: format.Alignment = StringAlignment.Far; format.LineAlignment = StringAlignment.Center; break;
                case 7: format.Alignment = StringAlignment.Near; format.LineAlignment = StringAlignment.Far; break;
                case 8: format.Alignment = StringAlignment.Center; format.LineAlignment = StringAlignment.Far; break;
                case 9: format.Alignment = StringAlignment.Far; format.LineAlignment = StringAlignment.Far; break;
                default: format.Alignment = StringAlignment.Center; format.LineAlignment = StringAlignment.Center; break;
            }
            return format;
        }

        /// <summary>
        /// 透明度
        /// </summary>
        /// <param name="opcity"></param>
        /// 0-100 ， 100为不透明
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>

        private System.Drawing.Imaging.ImageAttributes GetAlphaImgAttr(int opcity)
        {
            if (opcity < 0 || opcity > 100)
            {
                throw new ArgumentOutOfRangeException("opcity should be 0 -100");

            }

            float[][] matrixItems =
            {
                new float[] {1,0,0,0,0 },
                  new float[] {0,1,0,0,0 },
                    new float[] {0,0,1,0,0 },
                    new float[] {0,0,0,(float) opcity/100,0},
                      new float[] {0,0,0,0,1 }
          };

            System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(matrixItems);
            System.Drawing.Imaging.ImageAttributes imageAtt = new System.Drawing.Imaging.ImageAttributes();
            imageAtt.SetColorMatrix(colorMatrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

            return imageAtt;
        }

        private Rectangle GetRotateRectangle(int width, int height, float angle)
        {
            double radian = angle * Math.PI / 180;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);
            int newWidth = (int)(Math.Max(Math.Abs(width * cos - height * sin), Math.Abs(width * cos + height * sin)));
            int newHeight = (int)(Math.Max(Math.Abs(width * sin - height * cos), Math.Abs(width * sin + height * cos)));
            return new Rectangle(0, 0, newWidth, newHeight);
        }

        private Bitmap GetRotateImage(Bitmap src, int angle)
        {
            angle = angle % 360;
            int srcWidth = src.Width;
            int srcHeight = src.Height;

            Rectangle rotateRec = GetRotateRectangle(srcWidth, srcHeight, angle);

            int rotateWidth = rotateRec.Width;
            int rotateHeight = rotateRec.Height;

            Bitmap dest = null;
            Graphics graphics = null;

            try
            {
                dest = new Bitmap(rotateWidth, rotateHeight);
                graphics = Graphics.FromImage(dest);
                Point centerPoint = new Point(rotateWidth / 2, rotateHeight / 2);
                graphics.TranslateTransform(centerPoint.X, centerPoint.Y);
                graphics.RotateTransform(angle);
                graphics.TranslateTransform(-centerPoint.X, -centerPoint.Y);
                Point offset = new Point((rotateWidth - srcWidth) / 2, (rotateHeight - srcHeight) / 2);
                graphics.DrawImage(src, new Rectangle(offset.X, offset.Y, srcWidth, srcHeight));
                graphics.ResetTransform();
                graphics.Save();

            }
            catch
            {

            }
            finally
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                }
            }
            return dest;
        }


        private Bitmap GetTextBitmap(string text, int angle)
        {
            using (Bitmap bitmap = new Bitmap(400, 200))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    using (Font f = new Font("Segoe UI", 28, FontStyle.Bold, GraphicsUnit.Pixel))
                    {
                        SizeF size = g.MeasureString(text, f);
                        int fontWidth = (int)Math.Ceiling(size.Width) + 2;
                        int fontHeight = (int)Math.Ceiling(size.Height) + 2;

                        using (Bitmap bitmaptxt = new Bitmap(fontWidth, fontHeight))
                        {
                            using (var gf = Graphics.FromImage(bitmaptxt))
                            {
                                gf.Clear(Color.Transparent);
                                gf.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                gf.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                gf.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                                Rectangle validRect = new Rectangle(1, 1, fontWidth, fontHeight);
                                using (Brush b = new SolidBrush(Color.Red))
                                {
                                    using (Bitmap transImg = new Bitmap(bitmap))
                                    {
                                        using (Graphics gForTransImg = Graphics.FromImage(transImg))
                                        {
                                            gForTransImg.DrawString(text, f, b, validRect, GetStringFormat(5));

                                            System.Drawing.Imaging.ImageAttributes imageAtt = GetAlphaImgAttr(6);
                                            gf.DrawImage(transImg, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, transImg.Width, transImg.Height, GraphicsUnit.Pixel, imageAtt);
                                            gf.Save();
                                        }
                                    }
                                }
                            }
                            return GetRotateImage(bitmaptxt, 330);
                        }
                    }
                }
            }
        }



        public void GenDetailReport(string reportname, string reportfolder, ReportItemDetail detail, int index, IScript script, ScriptEventArgs eventargs)
        {
            lock (obj)
            {
                Validate(reportfolder);
            }

            string detailfoldername = System.IO.Path.GetFileNameWithoutExtension(reportname);

            var assembly = Assembly.GetExecutingAssembly();
            var detailresourceStream = assembly.GetManifestResourceStream("AutoFrameWork.Report.log.html");
            Stream pic404 = null;
            string detailcontent = string.Empty;
            using (var reader = new System.IO.StreamReader(detailresourceStream, System.Text.Encoding.UTF8))
            {
                detailcontent = reader.ReadToEnd();
            }

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
            detail.Name = detailreportname;
            lock (obj)
            {

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


            }


            string detailreport = string.IsNullOrEmpty(reportfolder) ? $"{detailfoldername}\\{detailreportname}_{detail.Status.ToString().ToLower()}.html" : $"{reportfolder}\\{detailfoldername}\\{detailreportname}_{detail.Status.ToString().ToLower()}.html";
            detail.Path = $"{detailfoldername}\\{detailreportname}_{detail.Status.ToString().ToLower()}.html";
            string logMessage = string.Empty;
            System.Text.StringBuilder logsb = new System.Text.StringBuilder();

            if(detail.Log != null)
            {
                int totalpics = detail.Log.Where(t => t.LogType == LogType.Picture).Count();
                int indexpic = 1;
                foreach(var log in detail.Log)
                {
                    if(log.LogType==LogType.StringContent)
                    {
                        string logcontent = log.Content?.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                        if(log.Style==Style.Default)
                        {
                            logsb.Append(logcontent);
                        }
                        else
                        {
                            string color = log.Style == Style.Yellow ? "background-color:#ead158!important;float:left" : "background-color:#6da4ea!important;float:left;";
                            logsb.Append($"<div style=\"{color}\">{logcontent}</div>");
                        }
                    }
                    else if(log.LogType==LogType.HtmlContent)
                    {
                        logsb.AppendLine(log.Content);
                    }
                    else if (log.LogType== LogType.ZeroSpace)
                    {
                        logsb.Append($"<div class=\"assert\">&#8203</div>");
                    }
                    else
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.AppendLine(log.Dt);
                        sb.Append($"     {indexpic}/{totalpics} Pics     ");
                        string imagePath = string.Empty;
                        string base64url = string.Empty;
                        if(reportfolder!=null)
                        {
                            imagePath = $"{reportfolder}\\Pic\\{log.Content}";
                        }
                        else
                        {
                            imagePath = $"Pic\\{log.Content}";
                        }

                        using (System.IO.FileStream fs = new System.IO.FileStream(imagePath, System.IO.FileMode.Open))
                        {
                            string picmd5hash = string.Empty;
                            byte[] picbyte = new byte[fs.Length];
                            fs.Read(picbyte, 0, picbyte.Length);

                            using (var crypto = System.Security.Cryptography.MD5.Create())
                            {
                                var data = crypto.ComputeHash(picbyte);
                                picmd5hash = BitConverter.ToString(data).Replace("-", "").ToLower();
                            }

                            if(picmd5hash !=log.Hash)
                            {
                              if(pic404 ==null)
                                {
                                    pic404 = assembly.GetManifestResourceStream("AutoFrameWork.Report.404pic.png");
                                }
                              pic404.Seek(0, SeekOrigin.Begin);
                                byte[] bytedata = new byte[pic404.Length];
                                pic404.Read(bytedata, 0, bytedata.Length);
                                base64url=Convert.ToBase64String(bytedata);


                                Console.WriteLine("Original image cannot be found");
                                indexpic++;
                                continue;
                            }
                            else
                            {
                                fs.Seek(0,System.IO.SeekOrigin.Begin);
                                using (Bitmap srcbitmap= new Bitmap(fs))
                                {
                                    using (Bitmap txtbitmap = GetTextBitmap(sb.ToString(),330))
                                    {
                                        using (TextureBrush txbrus = new TextureBrush(txtbitmap))
                                        {
                                            txbrus.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
                                            using (Graphics g = Graphics.FromImage(srcbitmap))
                                            {
                                                g.FillRectangle(txbrus,new Rectangle(0,0,srcbitmap.Width-1,srcbitmap.Height-1));
                                                g.Flush();
                                            }
                                        }
                                    }
                                    using (System.IO.MemoryStream ms = new MemoryStream ())
                                    {
                                        srcbitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        ms.Seek(0, System.IO.SeekOrigin.Begin);
                                        byte[] bytedata = new byte[ms.Length];
                                        ms.Read(bytedata,0,bytedata.Length);
                                        base64url = Convert.ToBase64String(bytedata);
                                    }
                                }

                            }
                        }
                        logsb.AppendLine($"<img src='data:image/png;base64,{base64url}' alt='' style='max-width:100%' />");
                            indexpic++;
                    }
                }
            }

            logMessage = logsb.ToString();
            if(string.IsNullOrEmpty(logMessage))
            {
                logMessage = "&lt;None&gt;";
            }
            string newdetailcontent = string.Empty;
            string bcolor = string.Empty;
            string batchmode = string.Empty;
           
            if(detail.Status==ScriptStatus.Pass)
            {
                bcolor = "#28a745";
            }
            else if (detail.Status== ScriptStatus.Fail)
            {
                bcolor = "#dc3545";

            }
            else
            {
                bcolor = "#ffc107";
            }
            if (IsBatchMode)
            {
                batchmode = "<span class=\"badges\">Batch Mode</span>";
            }

            string detailstatus = $"<span class=\"badges\" style=\"background-color:{bcolor}\">{detail.Status}</span>";

            newdetailcontent = detailcontent.Replace("#{Status}", detailstatus).Replace("#{Content}", logMessage);

            lock (obj)
            {
                using(var writer= System.IO.File.Create(detailreport))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(newdetailcontent);
                    writer.Write(bytes, 0, bytes.Length);
                }
            }

            if (_upload != null)
            {
                var upload = _upload.DeepCopy() as IResultUpload;
                try
                {
                    string displayname = script.GetType().GetCustomAttribute<ScriptAttribute>().DisplayName;

                    var sai = script.PropertyValue<DynamicPropertyView>("AdditionalInfo");
                    int duration = detail.Duration.Days * 24 * 60 * 60 + detail.Duration.Hours * 60 * 60 + detail.Duration.Minutes * 60 + detail.Duration.Seconds;
                    sai.SetMember("Duration", duration.ToString());
                    if (!sai.Keys.Contains("BatchMode"))
                    {
                        sai.SetMember("BatchMode", "Y");
                    }
                    bool needupload = upload.NeedUpload(displayname, detailreport, script.PropertyValue<string>("CaseId"), script.PropertyValue<string>("ReportName"), script.PropertyValue<string>("ScriptId"), detail.Status, sai);
                    if (needupload)
                    {
                        upload.Upload();
                        detail.UpStatus = UploadStatus.Pass;
                    }
                    else
                    {
                        detail.UpStatus = UploadStatus.NotEnabled;
                    }
                }
                catch (System.Exception e)
                {
                    detail.UpStatus = UploadStatus.Error;
                    detail.UpErrorMessage = e.Message;
                    eventargs.UploadMessage = e.Message;
                }
            
            }else
            {
                detail.UpStatus=UploadStatus.NotEnabled;
            }
            eventargs.Upload = detail.UpStatus;
        }

        public void GenReport(string reportname, string reportfolder)
        {
            Validate(reportfolder);
            string detailfoldername = System.IO.Path.GetFileNameWithoutExtension(reportname);
            // Console.WriteLine(detailfoldername);
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream("AutoFrameWork.Report.report.html");
        
            string content = string.Empty;
            string detailcontent = string.Empty;

            using (var reader = new System.IO.StreamReader(resourceStream, System.Text.Encoding.UTF8))
            {
                content = reader.ReadToEnd();

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

                int failuploadcount = item.Details.Where(t=>t.UpStatus==UploadStatus.Error).Count();
                string failupload = failuploadcount == 0 ? string.Empty : $"{failuploadcount} failed";

                sb.AppendLine($"<tr class='dataEntity'>");
                sb.AppendLine($"<td>{item.Name}</td>");
                sb.AppendLine($"<td></td>");
                sb.AppendLine($"<td></td>");
                sb.AppendLine($"<td class='text-center'>{item.Creator}</td>");
                sb.AppendLine($"<td class='text-center'>{item.Count}</td>");
                sb.AppendLine($"<td class='text-center'>{item.Pass}</td>");
                sb.AppendLine($"<td class='text-center'>{item.Fail}</td>");
                sb.AppendLine($"<td class='text-center'>{item.Error}</td>");
                sb.AppendLine($"<td class='text-center'>{failupload}</td>");
                sb.AppendLine($"<td class='text-center'>");
                sb.AppendLine($"<a href='javascript:void(0)' class='detail'>Show</a>");
                sb.AppendLine($"</td>");
                sb.AppendLine($"</tr>");
                int index = 0;
                foreach (var detail in item.Details)
                {
                    index++;
                    key++;

                  
                    sb.AppendLine($"<tr data-level={detail.Status} class='hiddenRow'>");
                    if (detail.Status==ScriptStatus.Pass)
                    {
                        sb.AppendLine($"<td class='passCase'>");
                    }
                    else
                    {
                        sb.AppendLine($"<td class='failCase'>");
                    }
                    sb.AppendLine($"<div class='testcase' data-toggle='modal' data-target='#dataModal' data-whatever='key{key}'>{detail.Name}</div>");
                    mapping.Add($"key{key}", detail.Data);

                    sb.AppendLine($"</td>");
                    sb.AppendLine($"<td align='center'>{detail.CaseId}</td>");
                    sb.AppendLine($"<td align='center'>{detail.Duration.Days * 24 * 60 * 60 + detail.Duration.Hours * 60 * 60 + detail.Duration.Minutes * 60 + detail.Duration.Seconds}s</td>");
                    sb.AppendLine($"<td colspan='4' align='center'>");
                    if (detail.Status== ScriptStatus.Pass)
                    {
                        sb.AppendLine($"<span class='label label-success success'>pass</span>");
                    }
                    else
                    {
                        sb.AppendLine($"<span  class='btn btn-danger btn-xs' data-toggle='collapse' data-target='#div_ft{rownum}_{index}'>{detail.Status.ToString().ToLower()}</span>");
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
                 

              if(UploadStatus.Error != detail.UpStatus)
                    {
                        sb.AppendLine($"<span class='label label-success success'>{detail.UpStatus.GetDesc().ToLower()}</span>");
                    }
              else
                    {
                        sb.AppendLine($"<span  class=\"btn btn-danger btn-xs\" data-toggle=\"collapse\" data-target='#div_up{rownum}_{index}'>{detail.UpStatus.GetDesc().ToLower()}</span>");
                        sb.AppendLine($" <div id='div_up{rownum}_{index}' class=\"collapse in\">");
                        sb.AppendLine("<pre>");
                        string errormessage = detail.UpErrorMessage;
                        errormessage = errormessage.Replace("<", "&lt;").Replace(">","&gt;");
                        sb.AppendLine(errormessage);
                        sb.AppendLine("</pre>");
                        sb.AppendLine("</div>");
                        
                    }
                    sb.AppendLine("</td>");
                    sb.AppendLine($"<td align='center'>");
                    string detailreport = detail.Path;


                    sb.AppendLine($"<a href='{detailreport}'  class='detail'>View Log</a>");
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


        //public void GenBatchReport(string reportname, string reportfolder)
        //{
        //    Validate(reportfolder);
        //    string detailfoldername = System.IO.Path.GetFileNameWithoutExtension(reportname);
        //    // Console.WriteLine(detailfoldername);
        //    var assembly = Assembly.GetExecutingAssembly();
        //    var resourceStream = assembly.GetManifestResourceStream("AutoFrameWork.Report.report.html");
        //    var detailresourceStream = assembly.GetManifestResourceStream("AutoFrameWork.Report.log.html");
        //    string content = string.Empty;
        //    string detailcontent = string.Empty;

        //    using (var reader = new System.IO.StreamReader(resourceStream, System.Text.Encoding.UTF8))
        //    {
        //        content = reader.ReadToEnd();

        //    }
        //    using (var reader = new System.IO.StreamReader(detailresourceStream, System.Text.Encoding.UTF8))
        //    {
        //        detailcontent = reader.ReadToEnd();

        //    }

        //    int total = 0;
        //    int pass = 0;
        //    int fail = 0;
        //    int error = 0;
        //    string replaceContent = string.Empty;
        //    int rownum = 0;
        //    int key = 0;
        //    Dictionary<string, string> dict = new Dictionary<string, string>();
        //    Dictionary<string, AFWDictionary<string>> mapping = new Dictionary<string, AFWDictionary<string>>();
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    foreach (var item in TestCases)
        //    {
        //        rownum++;
        //        total += item.Count;
        //        pass += item.Pass;
        //        fail += item.Fail;
        //        error += item.Error;

        //        sb.AppendLine($"<tr class='dataEntity'>");
        //        sb.AppendLine($"<td>{item.Name}</td>");
        //        sb.AppendLine($"<td></td>");
        //        sb.AppendLine($"<td></td>");
        //        sb.AppendLine($"<td class='text-center'>{item.Count}</td>");
        //        sb.AppendLine($"<td class='text-center'>{item.Pass}</td>");
        //        sb.AppendLine($"<td class='text-center'>{item.Fail}</td>");
        //        sb.AppendLine($"<td class='text-center'>{item.Error}</td>");
        //        sb.AppendLine($"<td class='text-center'>");
        //        sb.AppendLine($"<a href='javascript:void(0)' class='detail'>Show</a>");
        //        sb.AppendLine($"</td>");
        //        sb.AppendLine($"</tr>");
        //        int index = 0;
        //        if (item.Details.Count > 0)
        //        {
        //            var lastlog = item.Details.LastOrDefault()?.Log;
        //            if (lastlog != null)
        //            {
        //                int dividentifyflag = -1;
        //                System.Text.StringBuilder logsb = new System.Text.StringBuilder();
        //                foreach (var log in lastlog)
        //                {
        //                    if (log.LogType == LogType.StringContent)
        //                    {
        //                        if (log.Section != null)
        //                        {
        //                            if (dividentifyflag > -1)
        //                            {
        //                                logsb.AppendLine($"</div>");
        //                            }
        //                            logsb.AppendLine($"<div id=\"d{log.Section.Value}\">");
        //                            dividentifyflag = log.Section.Value;
        //                            continue;
        //                        }
        //                        string logcontent = log.Content?.Replace("<", "&lt;").Replace(">", "&gt;");
        //                        if (log.Style == Style.Default)
        //                        {
        //                            logsb.Append(logcontent);
        //                        }
        //                        else
        //                        {
        //                            string color = log.Style == Style.Yellow ? "background-color:#ead158!important;float:left" : "background-color: #6da4ea!important;float:left;";

        //                            logsb.Append($"<div style=\"{color}\">{logcontent}</div>");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        string imagePath = string.Empty;
        //                        string base64url = string.Empty;
        //                        if (reportfolder != null)
        //                        {
        //                            imagePath = $"{reportfolder}\\Pic\\{log.Content}";
        //                        }
        //                        else
        //                        {
        //                            imagePath = $"Pic\\{log.Content}";
        //                        }
        //                        using (System.IO.FileStream fs = new System.IO.FileStream(imagePath, System.IO.FileMode.Open))
        //                        {
        //                            byte[] byteData = new byte[fs.Length];
        //                            fs.Read(byteData, 0, byteData.Length);
        //                            base64url = Convert.ToBase64String(byteData);
        //                        }


        //                        logsb.AppendLine($"<img src='data:image/png;base64,{base64url}' alt='' style='max-width: 100%;' />");


        //                    }
        //                }
        //                if (dividentifyflag != -1)
        //                {
        //                    logsb.AppendLine($"</div>");
        //                }





        //                string logMessage = logsb.ToString();
        //                if (string.IsNullOrEmpty(logMessage))
        //                {
        //                    logMessage = "&lt;None&gt;";
        //                }
        //                string newdetailcontent = string.Empty;

        //                newdetailcontent = detailcontent.Replace("#{Content}", logMessage);
        //                if (string.IsNullOrEmpty(reportfolder))
        //                {

        //                    if (!System.IO.Directory.Exists(detailfoldername))
        //                    {
        //                        System.IO.Directory.CreateDirectory(detailfoldername);
        //                    }


        //                }
        //                else
        //                {
        //                    if (!System.IO.Directory.Exists(reportfolder))
        //                    {
        //                        System.IO.Directory.CreateDirectory(reportfolder);
        //                    }

        //                    if (!System.IO.Directory.Exists($"{reportfolder}\\{detailfoldername}"))
        //                    {
        //                        System.IO.Directory.CreateDirectory($"{reportfolder}\\{detailfoldername}");
        //                    }


        //                }
        //                string detailreport = string.IsNullOrEmpty(reportfolder) ? $"{detailfoldername}\\{item.Name}_testData.html" : $"{reportfolder}\\{detailfoldername}\\{item.Name}_testData.html";
        //                // Console.WriteLine(detailreport);
        //                using (var writer = System.IO.File.Create(detailreport))
        //                {
        //                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(newdetailcontent);
        //                    writer.Write(bytes, 0, bytes.Length);
        //                }


        //            }

        //        }

        //        foreach (var detail in item.Details)
        //        {

        //            key++;
        //            sb.AppendLine($"<tr data-level={detail.Status} class='hiddenRow'>");
        //            if ("Pass" == detail.Status)
        //            {
        //                sb.AppendLine($"<td class='passCase'>");
        //            }
        //            else
        //            {
        //                sb.AppendLine($"<td class='failCase'>");
        //            }
        //            sb.AppendLine($"<div class='testcase' data-toggle='modal' data-target='#dataModal' data-whatever='key{key}'>testData{index}</div>");
        //            mapping.Add($"key{key}", detail.Data);

        //            sb.AppendLine($"</td>");
        //            sb.AppendLine($"<td align='center'>{detail.Country}</td>");
        //            sb.AppendLine($"<td align='center'>{detail.CaseId}</td>");
        //            sb.AppendLine($"<td colspan='4' align='center'>");
        //            if ("Pass" == detail.Status)
        //            {
        //                sb.AppendLine($"<span class='label label-success success'>pass</span>");
        //            }
        //            else
        //            {
        //                sb.AppendLine($"<span  class='btn btn-danger btn-xs' data-toggle='collapse' data-target='#div_ft{rownum}_{index}'>{detail.Status.ToLower()}</span>");
        //                sb.AppendLine($"<div id='div_ft{rownum}_{index}' class='collapse in'>");
        //                sb.AppendLine($"<pre>");
        //                string errormessage = detail.ErrorMessage;
        //                errormessage = errormessage.Replace("<", "&lt;");
        //                errormessage = errormessage.Replace(">", "&gt;");
        //                sb.AppendLine(errormessage);
        //                sb.AppendLine($"</pre>");
        //                sb.AppendLine($"</div>");

        //            }
        //            sb.AppendLine($"</td>");
        //            sb.AppendLine($"<td align='center'>");



        //            sb.AppendLine($"<a href='{detailfoldername}\\{item.Name}_testData.html#d{index}'  class='detail'>View Log</a>");
        //            sb.AppendLine($"</td>");
        //            sb.AppendLine($"</tr>");
        //            index++;
        //        }


        //    }
        //    decimal rate = Math.Round((decimal)pass / (decimal)total * (decimal)100, 2);
        //    content = content.Replace("#{mapping}", Newtonsoft.Json.JsonConvert.SerializeObject(mapping));
        //    // content = content.Replace("#{data}", Newtonsoft.Json.JsonConvert.SerializeObject(dict));
        //    content = content.Replace("#{Content}", sb.ToString());
        //    content = content.Replace("#{Start Time}", StartTime.ToString());
        //    content = content.Replace("#{Duration}", Duration.ToString());
        //    content = content.Replace("#{Status}", $"All:{total} Pass:{pass} Failure:{fail} Error:{error}, Passing rate:{rate}%");
        //    content = content.Replace("#{Summary}", $"{rate}%");
        //    content = content.Replace("#{Failed}", fail.ToString());
        //    content = content.Replace("#{Passed}", pass.ToString());
        //    content = content.Replace("#{All}", total.ToString());
        //    content = content.Replace("#{Error}", error.ToString());



        //    string report = string.IsNullOrEmpty(reportfolder) ? reportname : $"{reportfolder}\\{reportname}";
        //    using (var writer = System.IO.File.Create(report))
        //    {
        //        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
        //        writer.Write(bytes, 0, bytes.Length);
        //    }

        //}



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
        private object obj = new object();

        public string Creator
        {
            get; internal set;
        }
        public int Count
        {
            get; private set;
        } = 0;

        public void SetCount(int num)
        {
            lock(obj)
            {
                Count = num;
            }
        }

        public int Pass
        {
            get;private  set;
        } = 0;

        public void PassAdd()
        {
            lock (obj)
            {
                Pass++;
            }
        }
        public int Fail
        {
            get;private set;
        } = 0;

        public void FailAdd()
        {
            lock (obj)
            {
                Fail++;
            }
        }

        public void CountAdd()
        {
            lock (obj)
            {
               Count++;
            }
        }


        public void ErrorAdd()
        {
            lock (obj)
            {
                Error++;
            }
        }

        public int Error
        {
            get; private set;
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

         public string Path
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public AFWDictionary<string> Data
        {
            get; set;
        }

        public ScriptStatus Status
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

    

        public string CaseId
        {
            get; set;
        }

         public UploadStatus UpStatus
        {
            get; set;
        }


        public string UpErrorMessage

        {
            get; set;
        }

        public TimeSpan Duration
        {
            get; internal set;
        }


    }
}