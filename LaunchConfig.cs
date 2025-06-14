using AutoFrameWork.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoFrameWork
{
    public class LaunchConfig
    {
        public static LaunchConfig _config;
        public static object obj = new object();

        private Dictionary<Type, object> dictToInstances = new Dictionary<Type, object>();

        internal IResultUpload _upload = null;

        private LaunchConfig()
        {

        }

        private long _encoderQuality = 80L;

        public long EncoderQuality
        {
            get { return _encoderQuality; }
            set
            {
                if ((value < 75L || value > 100L) && value != 10086L)
                {
                    throw new System.Exception("Parameter settings not within the allowed range for EncoderQuality >=75L <=100L");
                }
                else
                {
                    _encoderQuality = value;
                }
            }
        }

        public string LogFolderPath
        {
            get; set;
        }

        public string DataFile
        {
            get; set;
        }
        public string SheetName
        {
            get; set;
        }

        public void EnableUpload(IResultUpload upload)
        {
            _upload = upload;
        }

        public event EventHandler<ScriptEventArgs> AfterScriptCompleted;
        internal void OnAfterScripotCompleted(ScriptEventArgs e)
        {
            try
            {
                AfterScriptCompleted?.Invoke(null, e);
            }
            catch { }

        }

        public RegistrationBuilder<T> Register<T>() where T : class
        {


            if (dictToInstances.Keys.Contains(typeof(T)))
            {
                throw new System.Exception($"Duplication of register type {typeof(T)}");
            }
            RegistrationBuilder<T> rb = new RegistrationBuilder<T>();
            dictToInstances.Add(typeof(T), rb);
            return rb;
        }


        internal object GetRB(Type type)
        {

            if (!dictToInstances.Keys.Contains(type))
            {
                return null;
            }

            return dictToInstances[type];



        }

        public static LaunchConfig GetInstance()
        {
            if (_config == null)
            {
                lock (obj)
                {
                    if (_config == null)
                        _config = new LaunchConfig();
                }
            }

            return _config;
        }

    }

    public class ScriptEventArgs : EventArgs
    {
        public string Name
        { get; internal set; }
        public string ScriptId
        {
            get; internal set;
        }

        public AFWDictionary<string> Data
        { get; internal set; }
        public ScriptStatus Status
        { get; internal set; }


        public UploadStatus Upload
        { get; internal set; }
        public string UploadMessage
        { get; internal set; }
        public string Message
        { get; internal set; }

        public string Trace
        { get; internal set; }

        public dynamic AdditionalInfo
        { get; internal set; }



    }


    public class RegistrationBuilder<T> where T : class
    {


        public Action<T, Log.Log> EndHandle
        {
            get; set;
        }

        public Action<T, Log.Log> ExceptionHandle
        {
            get; set;
        }

        public Func<T, Log.Log, Task> EndHandleAsync
        { get; set; }

        public Func<T, Log.Log, Task> ExceptionHandleAsync
        { get; set; }

    }

    public interface IResultUpload
    {
        bool NeedUpload(string scriptname, string reportpath, string caseid, string reportname, string scriptid, ScriptStatus status, dynamic additionalInfo);
        void Upload();
    }
}