using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection.Metadata;

namespace AutoFrameWork.Log
{
    public class Log : System.IDisposable
    {
        private static AsyncLocal<Log> _log= new AsyncLocal<Log>();
        private static object obj = new object();

        private long _encoderQuality;

        internal static Log GetLog(string fileName, string fileFolder)
        {
            lock (obj)
            {
                if (_log.Value == null || fileName != _log.Value.FileName)
                {
                    _log.Value = new Log(fileName, fileFolder);
                }
            }
            return _log.Value;

        }

        public static Log GetLog()
        {
            lock (obj)
            {
                return _log.Value;
            }
        }

        private List<LogStore> _store = new List<LogStore>();

        public List<LogStore> Stores
        {
            get
            {
                return _store;
            }
        }


        private string FileName
        {
            get; set;
        }
        private string FileFolder
        {
            get; set;
        }
        private Log(string fileName, string fileFolder)
        {
            FileName = fileName;
            FileFolder = fileFolder;
            _encoderQuality = LaunchConfig.GetInstance().EncoderQuality;
            if (!string.IsNullOrEmpty(fileFolder) && !System.IO.Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }
            if (string.IsNullOrEmpty(fileFolder))
            {
                FileFolder = ".";
            }

        }


        internal void SetSection(int section)
        {
            _store.Add(new LogStore{LogType=LogType.StringContent,Content=string.Empty,Section=section});
        }

        internal void Insert(string content)
        {
            _store.Insert(0,new LogStore{LogType=LogType.StringContent, Content=content});
        }

        internal void InsertHTML(string content)
        {
            _store.Insert(0, new LogStore { LogType = LogType.HtmlContent, Content = content });
        }


        internal void InsertZeroSpace()
        {
            _store.Add(new LogStore { LogType = LogType.ZeroSpace });
        }
        public void Info(string content)
        {
            _store.Add(new LogStore { LogType = LogType.StringContent, Content = content });
        }

        public void Info(string content, Style style)
        {
             _store.Add(new LogStore { LogType = LogType.StringContent, Content = content,Style=style });
        }

        private string GenrandomFilename()
        {
            string name = $"{Guid.NewGuid().ToString("N").Substring(0, 8)}_{System.DateTime.Now.ToString("yyyyMMddHHmmss")}.png"; ;
            if (System.IO.File.Exists(name))
            {
                name = GenrandomFilename();
            }

            return name;
        }
        public void Info(byte[] bytes)
        {
            string dt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK");
            string name = GenrandomFilename();
            string md5hash = string.Empty;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (Image image = System.Drawing.Image.FromStream(ms))
                {
                    ms.Flush();
                    if(_encoderQuality == 10086L || !image.RawFormat.Equals(ImageFormat.Png))
                    {
                        image.Save($"{FileFolder}\\Pic\\{name}",System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        bool usejpeg = false;
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0]= new EncoderParameter (System.Drawing.Imaging.Encoder.Quality,_encoderQuality);

                        var jepgencoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.FormatID == ImageFormat.Jpeg.Guid);
                        using (System.IO.MemoryStream outms = new MemoryStream ())
                        {
                            image.Save(outms,jepgencoder,encoderParameters);

                            byte[] bytearrayImage = outms.ToArray ();
                            if (bytearrayImage.Length < bytes.Length)
                            {
                                usejpeg = true;
                            }
                        }

                        if (usejpeg)
                        {
                            image.Save($"{FileFolder}\\Pic\\{name}", jepgencoder, encoderParameters);
                        }
                        else
                        {
                            image.Save($"{FileFolder}\\Pic\\{name}", System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                      


                }

            }
            using(System.IO.FileStream fs = new FileStream ($"{FileFolder}\\Pic\\{name}", System.IO.FileMode.Open))
            {
                using (var crypto = System.Security.Cryptography.MD5.Create())
                {
                    var data = crypto.ComputeHash(fs);
                    md5hash = BitConverter.ToString(data).Replace("-", "").ToLower();
                }
            }


            _store.Add(new LogStore { LogType = LogType.Picture, Content = name, Dt=dt, Hash=md5hash });
        }
        public void Dispose()
        {
            _store.Clear();
            _log = null;
        }
    }

    public class LogStore
    {
        internal int? Section
        {
            get;set;
        }
        public LogType LogType
        {
            get; set;
        }

        public string Content
        {
            get; set;
        }
        public string Dt
            { get; set; }
        public string Hash
            { get; set; }

        public Style Style
        {
            get; set;
        }=Style.Default;
    }
    public enum LogType
    {
        StringContent, Picture,HtmlContent, ZeroSpace
    }

    public enum Style
    {
       Default,Yellow, Blue
    }
}