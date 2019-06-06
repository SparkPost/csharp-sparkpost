using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using SparkPost.RequestSenders;

namespace SparkPost
{
    public class Templates : ITemplates
    {
        private readonly Client client;
        private readonly IRequestSender requestSender;
        private readonly DataMapper dataMapper;

        public Templates(Client client, RequestSender requestSender, DataMapper dataMapper)
        {
            this.client = client;
            this.requestSender = requestSender;
            this.dataMapper = dataMapper;
        }

        public async Task<CreateTemplateResponse> Create(Template template)
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/templates",
                Method = "POST",
                Data = dataMapper.ToDictionary(template)
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var results = JsonConvert.DeserializeObject<dynamic>(response.Content).results;
            return new CreateTemplateResponse()
            {
                Id = results.id,
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content
            };
        }

        public async Task<RetrieveTemplateResponse> Retrieve(string templateId, bool? draft = null)
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/templates/{templateId}",
                Method = "GET"
            };

            if (draft != null)
            {
                string DraftTF = draft.ToString().ToLower();
                request.Url += $"?draft={DraftTF}";
            }

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var results = JsonConvert.DeserializeObject<dynamic>(response.Content).results;
            
            Dictionary<string, string> Headers = new Dictionary<string, string>();
            if (results.content.headers != null)
            {
                foreach (var property in results.content.headers.GetType().GetProperties())
                {
                    Headers[property.Name] = (string)property.GetValue(results.content.headers);
                }
            }

            return new RetrieveTemplateResponse()
            {
                Id = results.id,
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
                Name = results.name,
                Description = results.description,
                Published = results.published,
                LastUpdateTime = results.last_update_time,
                LastUse = (results.last_use == null) ? null : results.last_use,
                Options = new TemplateOptions()
                {
                    ClickTracking = results.options.click_tracking,
                    OpenTracking = results.options.open_tracking,
                    Transactional = (results.options.transactional == null) ? null : results.options.transactional
                },
                TemplateContent = new TemplateContent()
                {
                    From = new Address()
                    {
                        Email = results.content.from.email,
                        Name = results.content.from.name
                    },
                    Subject = results.content.subject,
                    ReplyTo = (results.content.reply_to == null) ? null : results.content.reply_to,
                    Text = (results.content.text == null) ? null : results.content.text,
                    Html = (results.content.html == null) ? null : results.content.html,
                    Headers = Headers
                }
            };
        }

        public async Task<RetrieveTemplatesResponse> List()
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/templates",
                Method = "GET"
            };

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var results = JsonConvert.DeserializeObject<dynamic>(response.Content).results;

            var templates = new List<TemplateListItem>();
            foreach (var result in results)
                templates.Add(new TemplateListItem
                {
                    Id = result.id,
                    Name = result.name,
                    LastUpdateTime = result.last_update_time,
                    Description = result.description,
                    Published = result.published
                });

            return new RetrieveTemplatesResponse
            {
                Templates = templates,
                StatusCode = response.StatusCode,
                Content = response.Content,
                ReasonPhrase = response.ReasonPhrase
            };
        }

        public async Task<bool> Delete(string templateId)
        {
            var request = new Request
            {
                Url = $"api/{client.Version}/templates/{templateId}",
                Method = "DELETE"
            };

            var response = await requestSender.Send(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<PreviewTemplateResponse> Preview(string templateId,
            IDictionary<string, object> substitutionData, bool? draft = null)
        {
            var data = new Dictionary<string, object>
            {
                {
                    "substitution_data", substitutionData ?? new Dictionary<string, object>()
                }
            };
            var request = new Request
            {
                Url = $"api/{client.Version}/templates/{templateId}/preview",
                Method = "POST",
                Data = data
            };

            if (draft != null)
            {
                string DraftTF = draft.ToString().ToLower();
                request.Url += $"?draft={DraftTF}";
            }

            var response = await requestSender.Send(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new ResponseException(response);

            var results = JsonConvert.DeserializeObject<dynamic>(response.Content).results;

            Dictionary<string, string> Headers = new Dictionary<string, string>();
            if (results.headers != null)
            {
                foreach (var property in results.content.headers.GetType().GetProperties())
                {
                    Headers[property.Name] = (string)property.GetValue(results.content.headers);
                }
            }

            return new PreviewTemplateResponse()
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Content = response.Content,
                From = new Address()
                {
                    Email = results.from.email,
                    Name = results.from.name 
                },
                Subject = results.subject,
                ReplyTo = results.reply_to,
                Text = results.text,
                Html = results.html,
                Headers = Headers
            };
        }
    }
}
