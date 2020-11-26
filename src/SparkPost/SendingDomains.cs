using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using SparkPost.RequestSenders;
using SparkPost.Utilities;

namespace SparkPost
{
    public class SendingDomains : ISendingDomains
    {
        private readonly IClient client;
        private readonly IRequestSender requestSender;
        private readonly IDataMapper dataMapper;

        public SendingDomains(IClient client, IRequestSender requestSender, IDataMapper dataMapper)
        {
            this.client = client;
            this.requestSender = requestSender;
            this.dataMapper = dataMapper;
        }

        public async Task<ListSendingDomainResponse> List()
        {
            var request = new Request
            {
                Url = $"/api/{client.Version}/sending-domains", 
                Method = "GET"
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            return ListSendingDomainResponse.CreateFromResponse(response);
        }

        public async Task<CreateSendingDomainResponse> Create(SendingDomain sendingDomain)
        {
            var request = new Request
            {
                Url = $"/api/{client.Version}/sending-domains", 
                Method = "POST",
                Data = dataMapper.ToDictionary(sendingDomain)
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var result = Jsonification.DeserializeObject<dynamic>(response.Content).results;
            return result != null ? new CreateSendingDomainResponse
                {
                    ReasonPhrase = response.ReasonPhrase,
                    StatusCode = response.StatusCode,
                    Content = response.Content,
                    Domain = result.domain,
                    Dkim = Dkim.ConvertToDkim(result.dkim),
                }
                : null;
        }

        public async Task<UpdateSendingDomainResponse> Update(SendingDomain sendingDomain)
        {
            var request = new Request
            {
                Url = $"/api/{client.Version}/sending-domains/{sendingDomain.Domain}", 
                Method = "PUT",
                Data = dataMapper.ToDictionary(sendingDomain)
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var result = Jsonification.DeserializeObject<dynamic>(response.Content).results;
            return result != null ? new UpdateSendingDomainResponse
                {
                    ReasonPhrase = response.ReasonPhrase,
                    StatusCode = response.StatusCode,
                    Content = response.Content,
                    Domain = result.domain,
                    TrackingDomain = result.tracking_domain,
                    Dkim = Dkim.ConvertToDkim(result.dkim),
                }
                : null;
        }

        public async Task<GetSendingDomainResponse> Retrieve(string domain)
        {
            var request = new Request
            {
                Url = $"/api/{client.Version}/sending-domains/{domain}", 
                Method = "GET",
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);
            var result = Jsonification.DeserializeObject<dynamic>(response.Content).results;
            return new GetSendingDomainResponse
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
                SendingDomain = SendingDomain.ConvertToSendingDomain(result)
            };
        }

        public async Task<Response> Delete(string domain)
        {
            var request = new Request
            {
                Url = $"/api/{client.Version}/sending-domains/{domain}",
                Method = "DELETE",
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                response.StatusCode = HttpStatusCode.OK;
                response.ReasonPhrase = ReasonPhrases.GetReasonPhrase((int)response.StatusCode);
            }

            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);
            return response;
        }

        public async Task<VerifySendingDomainResponse> Verify(VerifySendingDomain verifySendingDomain)
        {
            var request = new Request
            {
                Url = $"/api/{client.Version}/sending-domains/{verifySendingDomain.Domain}/verify",
                Method = "POST",
                Data = dataMapper.ToDictionary(verifySendingDomain)
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var result = Jsonification.DeserializeObject<dynamic>(response.Content).results;

            return new VerifySendingDomainResponse
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
                Status = VerifySendingDomainStatus.ConvertToVerifySendingDomainStatus(result)
            };
        }
    }
}