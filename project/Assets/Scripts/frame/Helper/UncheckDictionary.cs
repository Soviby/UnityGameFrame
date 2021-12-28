using System.Collections;
using System.Collections.Generic;

 
    public class UncheckedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary
    {
        public new TValue this[TKey key]
        {
            get
            {
                if (this.ContainsKey(key))
                    return base[key];
                return default;
            }
            set
            {
                if (this.ContainsKey(key))
                    base[key] = value;
                else
                    this.Add(key, value);
            }
        }
    }
