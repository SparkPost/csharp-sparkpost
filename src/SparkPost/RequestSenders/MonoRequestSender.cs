using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace SparkPost.RequestSenders
{
    public class MonoRequestSender : IRequestSender
    {
        private readonly IClient client;
        private readonly IDataMapper dataMapper;

        public MonoRequestSender(IClient client, IDataMapper dataMapper)
        {
            this.client = client;
            this.dataMapper = dataMapper;
        }

        public virtual async Task<Response> Send(Request request)
        {
            using (var httpClient = client.CustomSettings.CreateANewHttpClient())
            {
                httpClient.BaseAddress = new Uri(client.ApiHost);
                httpClient.DefaultRequestHeaders.Add("Authorization", client.ApiKey);

                if (client.SubaccountId != 0)
                    httpClient.DefaultRequestHeaders.Add("X-MSYS-SUBACCOUNT",
                        client.SubaccountId.ToString(CultureInfo.InvariantCulture));

                var result = await GetTheResponse(request, httpClient);

                return new Response
                {
                    StatusCode = result.StatusCode,
                    ReasonPhrase = result.ReasonPhrase,
                    Content = await result.Content.ReadAsStringAsync()
                };
            }
        }

        protected virtual async Task<HttpResponseMessage> GetTheResponse(Request request, HttpClient httpClient)
        {
            return await new RequestMethodFinder(httpClient, dataMapper)
                .FindFor(request)
                .Execute(request);
        }
    }
}