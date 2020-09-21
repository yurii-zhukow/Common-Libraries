using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

using Newtonsoft.Json;


namespace YZ {


    public static partial class Helpers {

        public static T JsonDeserializeWithDefault<T>(string s, Func<T> deflt = null) {

            if (string.IsNullOrWhiteSpace(s)) return (deflt ?? (() => default(T)))();
            var res =  JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore});
            return res;
        }

        public static object JsonDeserializeToAny(string s) {

            if (string.IsNullOrWhiteSpace(s)) return null;
            try {
                return JsonConvert.DeserializeObject(s, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }

        public static string JsonSerialize(object v) {
            if (v == null) return "";
            try {
                return JsonConvert.SerializeObject(v);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                return "";
            }
        }

    }


}
