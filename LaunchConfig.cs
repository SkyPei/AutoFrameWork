using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFrameWork
{
    public class LaunchConfig
    {
        public static LaunchConfig _config;
        public static object obj = new object();

        private Dictionary<Type, object> dictToInstances = new Dictionary<Type, object>();


        private LaunchConfig()
        {

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

    public class RegistrationBuilder<T> where T : class
    {
     
      
        public Action<T,Log.Log> EndHandle
        {
            get; set;
        }

        public Action<T,Log.Log> ExceptionHandle
        {
            get; set;
        }

    }
}