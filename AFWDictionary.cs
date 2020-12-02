using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ApiFrameWork
{
    public class AFWDictionary<TValue> : Dictionary<string, TValue>
    {
  

      public AFWDictionary() : base()
        {


        }
        public AFWDictionary(IEqualityComparer<string> comparer) : base(comparer)
        {

        }

     

        internal List<string> isWriten = new List<string>();

        public new TValue this[string key]
        {
            get
            {
                return base[key];

            }
            set
            {


                if (isWriten.FirstOrDefault(t => t.Contains(key, StringComparison.OrdinalIgnoreCase)) == null)
                {
                    isWriten.Add(key);
                }
               base[key]=value;
            }
        }
    }
}