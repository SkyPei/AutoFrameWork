using System;
using System.Reflection;
using ApiFrameWork.Command;
using ApiFrameWork.Utility;
using System.Linq;


namespace ApiFrameWork
{
    public class Launch
    {
        private LaunchConfig config;
       
       
        public Launch()
        {
            config = LaunchConfig.GetInstance();
          
        }

        public Launch Config(Action<LaunchConfig> doConfig)
        {
            doConfig.Invoke(config);
            return this;
        }
        
       
        public void Start()
        {

            
         
                var dataAccess = Assembly.GetCallingAssembly();
                var scripts = dataAccess.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IScript)) && t.BaseType==typeof(Script) ).ToList();
                CommandCenter.Startup(scripts, config.DataFile, config.SheetName);
           
        }
    }
}
