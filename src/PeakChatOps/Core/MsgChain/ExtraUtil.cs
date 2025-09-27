using System;
using System.Collections.Generic;
using System.Linq;

namespace PeakChatOps.Core.MsgChain
{
    public static class ExtraUtil
    {
        public static T GetExtraValue<T>(this IDictionary<string, object> extra, string key, T defaultValue = default)
        {
            if (extra == null) return defaultValue;
            if (!extra.TryGetValue(key, out var v) || v == null) return defaultValue;
            if (v is T t) return t;
            try
            {
                // handle arrays -> T[] cases or primitive conversions
                if (typeof(T).IsArray && v is Array arr)
                {
                    var elemType = typeof(T).GetElementType();
                    var casted = Array.CreateInstance(elemType, arr.Length);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        try { casted.SetValue(Convert.ChangeType(arr.GetValue(i), elemType), i); } catch { casted.SetValue(arr.GetValue(i), i); }
                    }
                    return (T)(object)casted;
                }
                return (T)Convert.ChangeType(v, typeof(T));
            }
            catch
            {
                try
                {
                    // last resort: attempt simple cast via LINQ for IEnumerable->array conversions
                    if (typeof(T).IsArray && v is System.Collections.IEnumerable ie)
                    {
                        var elemType = typeof(T).GetElementType();
                        var list = new List<object>();
                        foreach (var item in ie) list.Add(item);
                        var casted = Array.CreateInstance(elemType, list.Count);
                        for (int i = 0; i < list.Count; i++)
                        {
                            try { casted.SetValue(Convert.ChangeType(list[i], elemType), i); } catch { casted.SetValue(list[i], i); }
                        }
                        return (T)(object)casted;
                    }
                }
                catch { }
                return defaultValue;
            }
        }

        // Helper to try get a nested dictionary value and return as Dictionary<string, object>
        public static Dictionary<string, object> GetNestedDictionary(this IDictionary<string, object> extra, string key)
        {
            if (extra == null) return null;
            if (!extra.TryGetValue(key, out var v) || v == null) return null;
            if (v is Dictionary<string, object> dict) return dict;
            if (v is IDictionary<string, object> idict) return new Dictionary<string, object>(idict);
            if (v is ExitGames.Client.Photon.Hashtable ht)
            {
                // Lightweight conversion to Dictionary
                var result = new Dictionary<string, object>();
                foreach (System.Collections.DictionaryEntry de in ht)
                {
                    result[de.Key.ToString()] = de.Value;
                }
                return result;
            }
            return null;
        }
    }
}
