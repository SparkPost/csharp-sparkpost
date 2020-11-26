using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SparkPost.RequestSenders;
using SparkPost.Utilities;

namespace SparkPost
{
    public class Transmissions : ITransmissions
    {
        private readonly Client client;
        private readonly IRequestSender requestSender;
        private readonly DataMapper dataMapper;

        public Transmissions(Client client, IRequestSender requestSender, DataMapper dataMapper)
        {
            this.client = client;
            this.requestSender = requestSender;
            this.dataMapper = dataMapper;
        }

        public async Task<SendTransmissionResponse> Send(Transmission transmission)
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/transmissions",
                Method = "POST",
                Data = dataMapper.ToDictionary(transmission)
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) {
                throw new ResponseException(response);
            }

            var results = Jsonification.DeserializeObject<JsonSendResponse>(response.Content).Results;
            return new SendTransmissionResponse()
            {
                Id = results.Id,
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
                TotalAcceptedRecipients = results.TotalAcceptedRecipients,
                TotalRejectedRecipients = results.TotalRejectedRecipients,
            };
        }

        public async Task<RetrieveTransmissionResponse> Retrieve(string transmissionId)
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/transmissions/" + transmissionId,
                Method = "GET",
            };

            var response = await requestSender.Send(request);

            if (new[] {HttpStatusCode.OK, HttpStatusCode.NotFound}.Contains(response.StatusCode) == false)
                throw new ResponseException(response);

            var transmissionResponse = new RetrieveTransmissionResponse()
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
            };

            try
            {
                var results = Jsonification.DeserializeObject<dynamic>(response.Content).results;
                if (results.transmission == null) return transmissionResponse;

                transmissionResponse.Id = results.transmission.id;
                transmissionResponse.Description = results.transmission.description;
                transmissionResponse.GenerationEndTime = results.transmission.generation_end_time;
                transmissionResponse.RecipientListTotalChunks = results.transmission.rcpt_list_total_chunks ?? 0;
                transmissionResponse.RecipientListTotalSize = results.transmission.rcpt_list_chunk_size ?? 0;
                transmissionResponse.NumberRecipients = results.transmission.num_rcpts ?? 0;
                transmissionResponse.NumberGenerated = results.transmission.num_generated ?? 0;
            }
            catch
            {
                // ignored
            }

            return transmissionResponse;
        }

        public async Task<ListTransmissionResponse> List()
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/transmissions",
                Method = "GET",
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var transmissionResponse = new ListTransmissionResponse()
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
            };

            return transmissionResponse;
        }
    }

    internal class JsonSendResponse
    {
        [JsonProperty("results")]
        public JsonSendResult Results { get; set; }
    }

    internal class JsonSendResult
    {
        [JsonProperty("total_rejected_recipients")]
        public int TotalRejectedRecipients { get; set; }

        [JsonProperty("total_accepted_recipients")]
        public int TotalAcceptedRecipients { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }
    }
}