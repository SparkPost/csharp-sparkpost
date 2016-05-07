using System.Collections.Generic;
using Newtonsoft.Json;
using SparkPost.RequestSenders;
using System.Net;
using System.Threading.Tasks;

namespace SparkPost
{
    public class MessageEvents : IMessageEvents
    {
        private readonly IClient client;
        private readonly IRequestSender requestSender;
        private readonly IDataMapper dataMapper;

        public MessageEvents(IClient client, IRequestSender requestSender, IDataMapper dataMapper)
        {
            this.client = client;
            this.requestSender = requestSender;
            this.dataMapper = dataMapper;
        }

        public async Task<ListMessageEventsResponse> List(MessageEventsQuery messageEventsQuery)
        {
            return await List(messageEventsQuery as object);
        }

        public async Task<ListMessageEventsResponse> List(object messageEventsQuery = null)
        {
            if (messageEventsQuery == null) messageEventsQuery = new { };

            var request = new Request
            {
                Url = $"/api/{client.Version}/message-events",
                Method = "GET",
                Data = messageEventsQuery
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            //var listMessageEventsResponse = JsonConvert.DeserializeObject<ListMessageEventsResponse>(response.Content);
            //JsonConvert.DeserializeObject<>()

            dynamic results = response.StatusCode == HttpStatusCode.OK
                ? JsonConvert.DeserializeObject<dynamic>(response.Content).results
                : null;

            var listMessageEventsResponse = new ListMessageEventsResponse();
            listMessageEventsResponse.ReasonPhrase = response.ReasonPhrase;
            listMessageEventsResponse.StatusCode = response.StatusCode;
            listMessageEventsResponse.Content = response.Content;
            listMessageEventsResponse.MessageEvents = ConvertResultsToAListOfMessageEvents(results);

            return listMessageEventsResponse;
        }

        private static IEnumerable<MessageEvent> ConvertResultsToAListOfMessageEvents(dynamic results)
        {
            var messageEvents = new List<MessageEvent>();

            if (results == null) return messageEvents;

            foreach (var result in results)
            {
                var metadata =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        JsonConvert.SerializeObject(result.rcpt_meta));
                messageEvents.Add(new MessageEvent
                {
                    Type = result.type,
                    BounceClass = result.bounce_class,
                    CampaignId = result.campaign_id,
                    CustomerId = result.customer_id,
                    DeliveryMethod = result.delv_method,
                    DeviceToken = result.device_token,
                    ErrorCode = result.error_code,
                    IpAddress = result.ip_address,
                    MessageId = result.message_id,
                    MessageForm = result.msg_from,
                    MessageSize = result.msg_size,
                    NumberOfRetries = result.num_retries,
                    RecipientTo = result.rcpt_to,
                    RecipientType = result.rcpt_type,
                    RawReason = result.raw_reason,
                    Reason = result.reason,
                    RoutingDomain = result.routing_domain,
                    Subject = result.subject,
                    TemplateId = result.template_id,
                    TemplateVersion = result.template_version,
                    Timestamp = result.timestamp,
                    TransmissionId = result.transmission_id,
                    EventId = result.event_id,
                    FriendlyFrom = result.friendly_from,
                    IpPool = result.ip_pool,
                    QueueTime = result.queue_time,
                    RawRecipientTo = result.raw_rcpt_to,
                    SendingIp = result.sending_ip,
                    TDate = result.tdate,
                    Transactional = result.transactional,
                    RemoteAddress = result.remote_addr,
                    Metadata = metadata
                });
            }
            return messageEvents;
        }
    }
}