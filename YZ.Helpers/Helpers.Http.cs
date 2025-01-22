using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace YZ {


    public static partial class Helpers {
        public static async Task<T> GetJsonAsync<T>(this HttpClient http, string host, CancellationToken cancellationToken, params (string key, object value)[] args) {
            var q = args.ToString("&", kv => $"{HttpUtility.UrlEncode(kv.key)}{(kv.value == null ? "" : $"={HttpUtility.UrlEncode(kv.value.ToString())}")}");
            if (!string.IsNullOrWhiteSpace(q)) host = $"{host}?{q}";
            var res = await http.GetFromJsonAsync<T>(host, cancellationToken);
            return res;
        }
        public static async Task<T> GetJsonAsync<T>(this HttpClient http, string host, string path, CancellationToken cancellationToken, params (string key, object value)[] args) {
            if (!string.IsNullOrWhiteSpace(path)) host = $"{host.TrimEnd('/')}/{path.TrimStart('/')}";
            var q = args.Where(kv=> kv.value!=null).ToString("&", kv => $"{HttpUtility.UrlEncode(kv.key)}={HttpUtility.UrlEncode(kv.value.ToString())}");
            if (!string.IsNullOrWhiteSpace(q)) host = $"{host}?{q}";
            var res = await http.GetFromJsonAsync<T>(host, cancellationToken);
            return res;
        }
    }

}
