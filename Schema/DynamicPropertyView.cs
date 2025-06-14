using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFrameWork.Schema
{
    public class DynamicPropertyView : DynamicObject
    {
        private Dictionary<string, object> storage = new Dictionary<string, object>();
        private List<string> reservedkeys = new List<string> { "SetId", "Duratyion", "BatchMode" };


        public ICollection<string> Keys
        {
            get { return storage.Keys; }
        }

        public DynamicPropertyView()
        {
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (storage.ContainsKey(binder.Name))
            {
                result = storage[binder.Name];
                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string key = binder.Name;
            if (reservedkeys.Contains(key))
            {
                throw new System.Exception($"{key} is one of the reserved words");
            }
            else if (storage.ContainsKey(key))
            {
                storage[key] = value;
            }
            else
            {
                storage.Add(key, value);
            }

            return true;
        }

        internal void SetMember(string key, object value)
        {
            if(storage.ContainsKey(key))
            {
                storage[key] = value;
            }
            else
            {
                storage.Add(key, value);
            }
        }

        public override string ToString()
        {
            using (StringWriter message = new StringWriter())
            {
                foreach(var item in storage)
                {
                    message.WriteLine("{0}:\t{1}",item.Key,item.Value);
                }

                return message.ToString();
            }
        }

    }
}
