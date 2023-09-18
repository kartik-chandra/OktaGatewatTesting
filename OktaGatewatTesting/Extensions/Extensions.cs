using Newtonsoft.Json;
using System.Text;

namespace OktaGatewatTesting.Extensions
{
    public static class Extensions
    {
        public static StringContent ToJson(this object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }
    }
}
