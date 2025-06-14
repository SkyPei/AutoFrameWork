using System;
using System.Reflection;
using AutoFrameWork.Command;
using AutoFrameWork.Utility;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace AutoFrameWork
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


            var input = string.Empty;
            var dataAccess = Assembly.GetCallingAssembly();
            var scripts = dataAccess.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IScript)) && (t.BaseType == typeof(Script) || t.BaseType == typeof(ScriptAsync)) && true == t.GetCustomAttribute<Schema.ScriptAttribute>()?.Visible).ToList();
            CommandCenter.Startup(scripts, config, input);

        }
        public void Start(string[] args)
        {
            var input = args.Length==0?string.Empty: args[0];
            var dataAccess = Assembly.GetCallingAssembly();
            var scripts = dataAccess.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IScript)) && (t.BaseType == typeof(Script)|| t.BaseType== typeof(ScriptAsync)) && true == t.GetCustomAttribute<Schema.ScriptAttribute>()?.Visible).ToList();

            CommandCenter.Startup(scripts,config,input);
        }

        public void Start(string com,object input)
        {
            var dataAccess = Assembly.GetCallingAssembly();
            var scripts = dataAccess.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IScript)) && (t.BaseType == typeof(Script) || t.BaseType == typeof(ScriptAsync)) && true == t.GetCustomAttribute<Schema.ScriptAttribute>()?.Visible).ToList();

            CommandCenter.Startup(scripts,config,com,input);
        }

    }
}
