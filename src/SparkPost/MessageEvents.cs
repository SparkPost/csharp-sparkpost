using SparkPost.RequestSenders;
using SparkPost.Utilities;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SparkPost
{
    public class MessageEvents : IMessageEvents
    {
        private readonly IClient client;
        private readonly IRequestSender requestSender;

        public MessageEvents(IClient client, IRequestSender requestSender)
        {
            this.client = client;
            this.requestSender = requestSender;
        }

        public async Task<ListMessageEventsResponse> List()
        {
            return await List((MessageEventsQuery)null);
        }

        public async Task<ListMessageEventsResponse> List(MessageEventsQuery messageEventsQuery)
        {
            return await this.List($"/api/{client.Version}/events/message", messageEventsQuery);
        }

        public async Task<ListMessageEventsResponse> List(string url)
        {
            return await this.List(url, null);
        }

        public async Task<ListMessageEventsResponse> List(string url, MessageEventsQuery messageEventsQuery)
        {
            var request = new Request
            {
                Url = url,
                Method = "GET",
                Data = (object)messageEventsQuery ?? new { }
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            dynamic content = Jsonification.DeserializeObject<dynamic>(response.Content);

            var listMessageEventsResponse = new ListMessageEventsResponse
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
                MessageEvents = ConvertResultsToAListOfMessageEvents(content.results),
                TotalCount = content.total_count,
                Links = ConvertToLinks(content.links)
            };

            return listMessageEventsResponse;
        }

        public async Task<MessageEventSampleResponse> SamplesOf(string events)
        {
            var request = new Request
            {
                Url = $"/api/{client.Version}/events/message/samples?events={events}",
                Method = "GET"
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            return new MessageEventSampleResponse
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
            };
        }

        private static PageLink ConvertToLinks(dynamic page_links)
        {
            var links = new PageLink();

            if (page_links != null) links.Next = page_links.next;

            return links;
        }

        private static IEnumerable<MessageEvent> ConvertResultsToAListOfMessageEvents(dynamic results)
        {
            var messageEvents = new List<MessageEvent>();

            if (results == null) return messageEvents;

            foreach (var result in results)
            {
                var metadata =
                    Jsonification.DeserializeObject<Dictionary<string, string>>(
                        Jsonification.SerializeObject(result.rcpt_meta));
                var tags =
                    Jsonification.DeserializeObject<List<string>>(
                        Jsonification.SerializeObject(result.rcpt_tags));
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
                    MessageFrom = result.msg_from,
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
                    Transactional = result.transactional,
                    RemoteAddress = result.remote_addr,
                    Metadata = metadata,
                    TargetLinkUrl = result.target_link_url,
                    Tags = tags
                });
            }
            return messageEvents;
        }
    }
}