using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFrameWork.Utility
{
    internal static class ExtensionUtil
    {
        internal static object DeepCopy(this IResultUpload source)
        {
            var type = source.GetType();
            string jsonDatra = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject(jsonDatra,type);
        }

        internal static void PropertyAssignment(this IScript script , string property, object obj)
        {
             var scriptproperty = script.GetType().GetProperty(property);
            if (scriptproperty !=null)
            {
                scriptproperty.SetValue(script, obj);
            }
        }

        internal static T PropertyValue<T>(this IScript script , string property) where T:class
        {
            var scriptproperty= script.GetType().GetProperty(property);
            if(scriptproperty!=null)
            {
                return scriptproperty.GetValue(script) as T;
            }
            return null;
        }

        internal static Task<List<T>> ThreadPoolExecutionAsync<T,TSource>(this List<TSource>list, Func<TSource, T> func, int num)
        {
            return Task.Run<List<T>>(() =>
            {
                List<T> returnlist = new List<T>();
                int len = list.Count;
                int n = 0;
                List<Task<T>> tasks = new List<Task<T>>();
                for(;n<len && n<=num;n++)
                {
                    var content=list[n];
                    var task = Task.Factory.StartNew<T>(() => 
                    { 
                        return func.Invoke(content);
                    
                    },TaskCreationOptions.AttachedToParent);
                    tasks.Add(task);
                }
                while(n<len)
                {
                    Task.WaitAny(tasks.ToArray());

                    int tasklen = tasks.Where(t => !t.IsCompleted).Count();
                    while(n<len && tasklen<len)
                    {
                        var content = list[n];
                        var task = Task.Factory.StartNew<T>(() =>
                        {
                            return func.Invoke(content);
                        },TaskCreationOptions.AttachedToParent);
                        tasks.Add(task);
                        n++;
                    }
                }
                Task.WaitAll(tasks.ToArray());
                foreach (var item in tasks)
                {
                    returnlist.Add(item.GetAwaiter().GetResult());  
                }

                return returnlist;
            } );
        }

        internal static Task<List<T>> ThreadPoolExecutionAsync<T, TSource >(this List<TSource>list, Func<TSource,Task<T>>func,int num)
        {
            return Task.Run<List<T>>(() =>
            {
                List<T> returnlist = new List<T>();
                int len = list.Count;
                int n = 0;
                List<Task<T>> tasks = new List<Task<T>>();
                for (; n < len && n <= num; n++)
                {
                    var content = list[n];
                    var task = func.Invoke(content);
                    tasks.Add(task);
                }
                while (n < len)
                {
                    Task.WaitAny(tasks.ToArray());

                    int tasklen = tasks.Where(t => !t.IsCompleted).Count();
                    while (n < len && tasklen < len)
                    {
                        var content = list[n];
                        var task = func.Invoke(content);
                        tasks.Add(task);
                        n++;
                    }
                }
                Task.WaitAll(tasks.ToArray());
                foreach (var item in tasks)
                {
                    returnlist.Add(item.GetAwaiter().GetResult());
                }

                return returnlist;
            });
        }

        internal static Task ThreadPoolExecutionAsync<TSource>(this List<TSource> list, Action<TSource> act, int num)
        {
            return Task.Run(() =>
            {

                int len = list.Count;
                int n = 0;
                List<Task> tasks = new List<Task>();
                for (; n < len && n <= num; n++)
                {
                    var content = list[n];
                    var task = Task.Factory.StartNew(() =>
                    {
                       act.Invoke(content);

                    }, TaskCreationOptions.AttachedToParent);
                    tasks.Add(task);
                }
                while (n < len)
                {
                    Task.WaitAny(tasks.ToArray());

                    int tasklen = tasks.Where(t => !t.IsCompleted).Count();
                    while (n < len && tasklen < len)
                    {
                        var content = list[n];
                        var task = Task.Factory.StartNew(() =>
                        {
                            act.Invoke(content);
                        }, TaskCreationOptions.AttachedToParent);
                        tasks.Add(task);
                        n++;
                    }
                }
                Task.WaitAll(tasks.ToArray());
             
            });
        }

        internal static Task ThreadPoolExecutionAsync<TSource>(this List<TSource> list, Func<TSource, Task> act, int num)
        {
            return Task.Run(() =>
            {

                int len = list.Count;
                int n = 0;
                List<Task> tasks = new List<Task>();
                for (; n < len && n <= num; n++)
                {
                    var content = list[n];
                 var task=act.Invoke(content);
                    tasks.Add(task);
                }
                while (n < len)
                {
                    Task.WaitAny(tasks.ToArray());

                    int tasklen = tasks.Where(t => !t.IsCompleted).Count();
                    while (n < len && tasklen < len)
                    {
                        var content = list[n];
                        var task = act.Invoke(content);
                        tasks.Add(task);
                        n++;
                    }
                }
                Task.WaitAll(tasks.ToArray());

            });
        }
    }
}
