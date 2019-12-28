using System.Net.Http;
using System.Net.Http.Headers;

namespace GrandPrixAlmanac.Models
{
    public class APIHelper
    {
        public HttpClient ApiClient { get; set; }

        public void InitializeClient()
        {
            this.ApiClient = new HttpClient();

            this.ApiClient.DefaultRequestHeaders.Accept.Clear();
            this.ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
