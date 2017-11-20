using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SparkPost.RequestSenders;
using SparkPost.Utilities;

namespace SparkPost
{
    public class Suppressions : ISuppressions
    {
        private readonly IClient client;
        private readonly IRequestSender requestSender;
        private readonly IDataMapper dataMapper;

        public Suppressions(IClient client, IRequestSender requestSender, IDataMapper dataMapper)
        {
            this.client = client;
            this.requestSender = requestSender;
            this.dataMapper = dataMapper;
        }

        public async Task<ListSuppressionResponse> List(SuppressionsQuery supppressionsQuery)
        {
            return await List(supppressionsQuery as object);
        }

        public async Task<ListSuppressionResponse> List(object query = null)
        {
            if (query == null) query = new {};
            var request = new Request
            {
                Url = $"/api/{client.Version}/suppression-list",
                Method = "GET",
                Data = query
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var results = Jsonification.DeserializeObject<dynamic>(response.Content).results;

            return new ListSuppressionResponse
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
                Suppressions = ConvertResultsToAListOfSuppressions(results)
            };
        }

        public async Task<ListSuppressionResponse> Retrieve(string email)
        {
            var request = new Request
            {
                Url = $"/api/{client.Version}/suppression-list/{WebUtility.UrlEncode(email)}",
                Method = "GET"
            };

            var response = await requestSender.Send(request);

            if (new[] {HttpStatusCode.OK, HttpStatusCode.NotFound}.Contains(response.StatusCode) == false)
                throw new ResponseException(response);

            dynamic results = response.StatusCode == HttpStatusCode.OK
                ? Jsonification.DeserializeObject<dynamic>(response.Content).results
                : null;

            return new ListSuppressionResponse
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
                Suppressions = ConvertResultsToAListOfSuppressions(results)
            };
        }

        public async Task<UpdateSuppressionResponse> CreateOrUpdate(IEnumerable<string> emails)
        {
            var suppressions = emails.Select(email =>
                new Suppression
                {
                    Email = email,
                    Transactional = true,
                    NonTransactional = true
                });

            return await CreateOrUpdate(suppressions);
        }

        public async Task<UpdateSuppressionResponse> CreateOrUpdate(IEnumerable<Suppression> suppressions)
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/suppression-list",
                Method = "PUT JSON",
                Data = new
                {
                    recipients = suppressions.Select(x => dataMapper.ToDictionary(x)).ToList()
                }
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var updateSuppressionResponse = new UpdateSuppressionResponse();
            LeftRight.SetValuesToMatch(updateSuppressionResponse, response);
            return updateSuppressionResponse;
        }

        public async Task<bool> Delete(string email)
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/suppression-list/{WebUtility.UrlEncode(email)}",
                Method = "DELETE"
            };

            var response = await requestSender.Send(request);
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        private static IEnumerable<Suppression> ConvertResultsToAListOfSuppressions(dynamic results)
        {
            var suppressions = new List<Suppression>();

            if (results == null) return suppressions;

            foreach (var result in results)
            {
                suppressions.Add(new Suppression
                {
                    Description = result.description,
                    Transactional = result.transactional == true,
                    NonTransactional = result.non_transactional == true,
                    Email = result.recipient,
                    Source = result.source,
                    Created = result.created,
                    Updated = result.updated
                });
            }
            return suppressions;
        }

    }
}