using System.Collections.Generic;
using System.Net;

namespace AutoFrameWork.Log
{
    internal class ResponseContent: System.IDisposable
    {
        internal static ResponseContent _responseContent;
        private static object obj = new object();

        internal Dictionary<HttpWebResponse, string> List
        {
            get;set;
        }
        private ResponseContent()
        {
            List = new Dictionary<HttpWebResponse, string>();
        }

        internal static ResponseContent GetInstance()
        {

            if (_responseContent == null)
            {
                lock (obj)
                {
                    if (_responseContent == null)
                    {
                        _responseContent = new ResponseContent();
                    }
                }

            }

            return _responseContent;
        }

        public void Dispose()
        {
            List.Clear();
            
        }
    }
}