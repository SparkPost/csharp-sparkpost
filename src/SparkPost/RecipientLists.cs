using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SparkPost.RequestSenders;

namespace SparkPost
{
    public class RecipientLists : IRecipientLists
    {
        private readonly Client client;
        private readonly IRequestSender requestSender;
        private readonly DataMapper dataMapper;

        public RecipientLists(Client client, IRequestSender requestSender, DataMapper dataMapper)
        {
            this.client = client;
            this.requestSender = requestSender;
            this.dataMapper = dataMapper;
        }

        public async Task<RetrieveRecipientListsResponse> Retrieve(string recipientListsId)
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/recipient-lists/" + recipientListsId+ "?show_recipients=true",
                Method = "GET",
            };

            var response = await requestSender.Send(request);

            if (new[] {HttpStatusCode.OK, HttpStatusCode.NotFound}.Contains(response.StatusCode) == false)
                throw new ResponseException(response);

            var recipientListsResponse = new RetrieveRecipientListsResponse()
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
            };
            var results = JsonConvert.DeserializeObject<dynamic>(response.Content).results;
            if (results.recipients == null) return recipientListsResponse;

            recipientListsResponse.RecipientList = new RecipientList
            {
                Id = results.id,
                Name = results.name,
                Description = results.description,
                Attributes = results.attributes != null
                    ? new Attributes
                    {
                        InternalId = results.attributes.internal_id, ListGroupId = results.attributes.list_group_id
                    }
                    : null
            };
            recipientListsResponse.TotalAcceptedRecipients = results.total_accepted_recipients;
            ////recipientListsResponse.RecipientLists = RetrieveRecipientListsResponse.CreateFromResponse(response);

            ////recipientListsResponse.RecipientList = new RecipientList
            ////{
            ////    Id = recipientListsResponse.Id,
            ////    Recipients = recipientListsResponse.RecipientLists,
            ////    Attributes = recipientListsResponse.Attributes,
            ////    Description = recipientListsResponse.Description,
            ////    Name = recipientListsResponse.Name
            ////};

            return recipientListsResponse;
        }

        public async Task<bool> Delete(string id)
        {
            var request = new Request
            {
                Url = $"/api/{client.Version}/recipient-lists/{id}",
                Method = "DELETE"
            };

            var response = await requestSender.Send(request);
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public async Task<UpdateRecipientListResponse> Update(RecipientList recipientList)
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/recipient-lists/{recipientList.Id}",
                Method = "PUT",
                Data = dataMapper.ToDictionary(recipientList)
            };

            var response = await requestSender.Send(request);

            if (new[] {HttpStatusCode.OK, HttpStatusCode.NotFound}.Contains(response.StatusCode) == false)
                throw new ResponseException(response);

            return new UpdateRecipientListResponse()
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
            };
        }

        public async Task<SendRecipientListsResponse> Create(RecipientList recipientList)
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/recipient-lists?num_rcpt_errors=3",
                Method = "POST",
                Data = dataMapper.ToDictionary(recipientList)
            };

            var response = await requestSender.Send(request);

            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var results = JsonConvert.DeserializeObject<dynamic>(response.Content).results;
            return new SendRecipientListsResponse()
            {
                Id = results.id,
                TotalAcceptedRecipients = results.total_accepted_recipients,
                TotalRejectedRecipients = results.total_rejected_recipients,
                Name = results.name,
                Content = response.Content,
                StatusCode = response.StatusCode,
                ReasonPhrase = response.ReasonPhrase
            };
        }
    }
}