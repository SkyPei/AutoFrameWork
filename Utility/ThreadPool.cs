using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFrameWork.Utility
{
    internal class ThreadPool
    {
        private static readonly object obj = new object();

        private Dictionary<int, int> map;

        internal ThreadPool(int num)
        {
            map = new Dictionary<int, int>();
            for (int n = 0; n < num; n++)
            {
                map.Add(n, n);
            }
        }

        private int index = 0;

        internal List<Queue<BatchScriptModel>> DependencyQueues
            { get; set; } = new List<Queue<BatchScriptModel>>();
        internal Queue<BatchScriptModel> NonDepQueue { get; set; }= new Queue<BatchScriptModel>();

        internal BatchScriptModel Next(int i, bool isdiscard = false)
        {
            lock (obj)
            {
                int queindex = map[i];
                if(queindex >index)
                {
                    index = queindex;
                }

                if(isdiscard && queindex >=0)
                {
                    map[i] = ++index;
                    return Next(i);
                }
                else
                {
                    if(DependencyQueues.Count>0 && queindex <DependencyQueues.Count && queindex>=0)
                    {
                        var item = DependencyQueues[queindex];
                        if (item ==null || item.Count==0 )
                        {
                            map[i]= ++index;
                            return Next(i);
                        }
                        else
                        {
                            return item.Dequeue();
                        }
                    }
                    else
                    {
                        if(NonDepQueue.Count>0)
                        {
                            return NonDepQueue.Dequeue();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

    }


    internal class BatchScriptModel
    {
        internal int Id
        {
            get; set;
        }

        internal string Name { get; set; }
        internal string File {  get; set; }

        internal string Sheet { get; set; }
        internal int Index { get; set; }
        internal string SetId { get; set; }
        internal string ScriptId { get; set; }
    }
}
