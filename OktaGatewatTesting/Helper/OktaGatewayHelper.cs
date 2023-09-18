using Newtonsoft.Json;
using OktaGatewatTesting.Extensions;
using OktaGatewatTesting.Models;
using System.Net.Http.Headers;

namespace OktaGatewatTesting.Helper
{
    public class OktaGatewayHelper
    {
        public UserAuthTokenModel ValidateAuthenticationToken(string _baseUrl, string endPoint, string bearerToken, string resultToken, string sitename)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                    client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    //client.DefaultRequestHeaders.Add("Accept", "*/*");

                    var content = new { token = resultToken, sitename = sitename };
                    Task<HttpResponseMessage> response = client.PostAsync(_baseUrl + endPoint, content.ToJson());

                    Console.WriteLine($"_baseUrl + endPoint: {_baseUrl + endPoint} client:{client.ToString()}");
                    Console.WriteLine(response.Result.ToString());

                    if (response.Result.IsSuccessStatusCode)
                    {
                        var responseData = response.Result.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<UserAuthTokenModel>(responseData.Result.ToString());
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
