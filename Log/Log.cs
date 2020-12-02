using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Drawing;

namespace ApiFrameWork.Log
{
    public class Log : System.IDisposable
    {
        private static Log _log;
        private static object obj = new object();

        internal static Log GetLog(string fileName, string fileFolder)
        {
            lock (obj)
            {
                if (_log == null || fileName != _log.FileName)
                {
                    _log = new Log(fileName, fileFolder);
                }
            }
            return _log;

        }

        public static Log GetLog()
        {
            lock (obj)
            {
                return _log;
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
            string name = GenrandomFilename();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (Image image = System.Drawing.Image.FromStream(ms))
                {

                    image.Save($"{FileFolder}\\Pic\\{name}");


                }

            }
            _store.Add(new LogStore { LogType = LogType.Picture, Content = name });
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

        public Style Style
        {
            get; set;
        }=Style.Default;
    }
    public enum LogType
    {
        StringContent, Picture
    }

    public enum Style
    {
       Default,Yellow, Blue
    }
}