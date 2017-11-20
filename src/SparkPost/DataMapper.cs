using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using SparkPost.Utilities;
using SparkPost.ValueMappers;

namespace SparkPost
{
    public interface IDataMapper
    {
        IDictionary<string, object> ToDictionary(SendingDomain transmission);
        IDictionary<string, object> ToDictionary(Dkim dkim);
        IDictionary<string, object> ToDictionary(SendingDomainStatus sendingDomainStatus);
        IDictionary<string, object> ToDictionary(VerifySendingDomain verifySendingDomain);
        IDictionary<string, object> ToDictionary(Transmission transmission);
        IDictionary<string, object> ToDictionary(Recipient recipient);
        IDictionary<string, object> ToDictionary(Address address);
        IDictionary<string, object> ToDictionary(Options options);
        IDictionary<string, object> ToDictionary(Content content);
        IDictionary<string, object> ToDictionary(Attachment attachment);
        IDictionary<string, object> ToDictionary(InlineImage inlineImage);
        IDictionary<string, object> ToDictionary(File file);
        IDictionary<string, object> ToDictionary(Suppression suppression);
        IDictionary<string, object> ToDictionary(Webhook webhook);
        IDictionary<string, object> ToDictionary(Subaccount subaccount);
        IDictionary<string, object> ToDictionary(RelayWebhook relayWebhook);
        IDictionary<string, object> ToDictionary(InboundDomain inboundDomain);
        IDictionary<string, object> ToDictionary(RelayWebhookMatch relayWebhookMatch);
        IDictionary<string, object> CatchAll(object anything);
        object GetTheValue(Type propertyType, object value);
        IDictionary<Type, MethodInfo> ToDictionaryMethods();
        IDictionary<string, object> ToDictionary(RecipientList recipientList);
        IDictionary<string, object> ToDictionary(Template template);
        IDictionary<string, object> ToDictionary(TemplateContent templateContent);
        IDictionary<string, object> ToDictionary(TemplateOptions templateOptions);
        IDictionary<string, object> ToDictionary(MetricsQuery query);
    }

    public class DataMapper : IDataMapper
    {
        private readonly IEnumerable<IValueMapper> valueMappers;

        public DataMapper(string version = "v1")
        {
            valueMappers = new List<IValueMapper>
            {
                new MapASingleItemUsingToDictionary(this),
                new MapASetOfItemsUsingToDictionary(this),
                new BooleanValueMapper(),
                new EnumValueMapper(),
                new DateTimeOffsetValueMapper(),
                new DateTimeValueMapper(),
                new StringObjectDictionaryValueMapper(this),
                new StringStringDictionaryValueMapper(),
                new EnumerableValueMapper(this),
                new AnonymousValueMapper(this)
            };
        }

        public virtual IDictionary<string, object> ToDictionary(SendingDomain sendingDomain)
        {
            return WithCommonConventions(sendingDomain);
        }

        public virtual IDictionary<string, object> ToDictionary(SendingDomainStatus sendingDomainStatus)
        {
            return WithCommonConventions(sendingDomainStatus);
        }

        public virtual IDictionary<string, object> ToDictionary(Dkim dkim)
        {
            return WithCommonConventions(dkim);
        }

        public virtual IDictionary<string, object> ToDictionary(VerifySendingDomain verifySendingDomain)
        {
            return WithCommonConventions(verifySendingDomain);
        }

        public virtual IDictionary<string, object> ToDictionary(Transmission transmission)
        {
            var data = new Dictionary<string, object>
            {
                ["substitution_data"] = AlterThisDictionaryToBePassedToSparkpost(transmission.SubstitutionData),
                ["metadata"] = AlterThisDictionaryToBePassedToSparkpost(transmission.Metadata),
                ["recipients"] = transmission.ListId != null
                    ? (object) new Dictionary<string, object> {["list_id"] = transmission.ListId}
                    : transmission.Recipients.Select(ToDictionary)
            };

            var result = WithCommonConventions(transmission, data);

            CcHandling.Process(transmission, result);

            return result;
        }

        private static IDictionary<string, object> AlterThisDictionaryToBePassedToSparkpost(IDictionary<string, object> dictionary)
        {
            return dictionary != null && dictionary.Keys.Any()
                ? dictionary
                : null;
        }

        public virtual IDictionary<string, object> ToDictionary(Recipient recipient)
        {
            return WithCommonConventions(recipient, new Dictionary<string, object>()
            {
                ["type"] = null,
                ["substitution_data"] =
                    recipient.SubstitutionData != null && recipient.SubstitutionData.Keys.Any()
                        ? recipient.SubstitutionData
                        : null,
            });
        }

        public virtual IDictionary<string, object> ToDictionary(Suppression suppression)
        {
            return WithCommonConventions(suppression);
        }

        public virtual IDictionary<string, object> ToDictionary(Webhook webhook)
        {
            return WithCommonConventions(webhook);
        }

        public virtual IDictionary<string, object> ToDictionary(Address address)
        {
            return WithCommonConventions(address);
        }

        public virtual IDictionary<string, object> ToDictionary(Options options)
        {
            return AnyValuesWereSetOn(options) ? WithCommonConventions(options) : null;
        }

        public virtual IDictionary<string, object> ToDictionary(Content content)
        {
            return WithCommonConventions(content);
        }

        public virtual IDictionary<string, object> ToDictionary(Attachment attachment)
        {
            return ToDictionary(attachment as File);
        }

        public virtual IDictionary<string, object> ToDictionary(InlineImage inlineImage)
        {
            return ToDictionary(inlineImage as File);
        }

        public virtual IDictionary<string, object> ToDictionary(File file)
        {
            return WithCommonConventions(file);
        }

        public IDictionary<string, object> ToDictionary(Subaccount subaccount)
        {
            return WithCommonConventions(subaccount);
        }

        public IDictionary<string, object> ToDictionary(InboundDomain inboundDomain)
        {
            return WithCommonConventions(inboundDomain);
        }

        public IDictionary<string, object> ToDictionary(RelayWebhook relayWebhook)
        {
            return WithCommonConventions(relayWebhook);
        }

        public IDictionary<string, object> ToDictionary(RelayWebhookMatch relayWebhookMatch)
        {
            return WithCommonConventions(relayWebhookMatch);
        }

        public IDictionary<string, object> ToDictionary(MessageEventsQuery query)
        {
            return WithCommonConventions(query, new Dictionary<string, object>()
            {
                ["events"] = string.Join(",", query.Events),
                ["campaign_ids"] = string.Join(",", query.CampaignIds),
                ["bounce_classes"] = string.Join(",", query.BounceClasses),
                ["campaign_ids"] = string.Join(",", query.CampaignIds),
                ["friendly_froms"] = string.Join(",", query.FriendlyFroms),
                ["message_ids"] = string.Join(",", query.MessageIds),
                ["recipients"] = string.Join(",", query.Recipients),
                ["subaccounts"] = string.Join(",", query.Subaccounts),
                ["template_ids"] = string.Join(",", query.TemplateIds),
                ["transmission_ids"] = string.Join(",", query.TransmissionIds)
            });
        }

        public IDictionary<string, object> ToDictionary(MetricsQuery query)
        {
            return WithCommonConventions(query, new Dictionary<string, object>()
            {
                ["domains"] = string.Join(",", query.Domains),
                ["campaigns"] = string.Join(",", query.Campaigns),
                ["templates"] = string.Join(",", query.Templates),
                ["sending_ips"] = string.Join(",", query.SendingIps),
                ["ip_pools"] = string.Join(",", query.IpPools),
                ["sending_domains"] = string.Join(",", query.SendingDomains),
                ["subaccounts"] = string.Join(",", query.Subaccounts),
                ["metrics"] = string.Join(",", query.Metrics)
            });
        }

        public virtual IDictionary<string, object> ToDictionary(Template template)
        {
            return WithCommonConventions(template);
        }

        public virtual IDictionary<string, object> ToDictionary(TemplateContent templateContent)
        {
            return WithCommonConventions(templateContent);
        }

        public virtual IDictionary<string, object> ToDictionary(TemplateOptions templateOptions)
        {
            return WithCommonConventions(templateOptions);
        }

        public IDictionary<string, object> CatchAll(object anything)
        {
            var converters = ToDictionaryMethods();
            if (converters.ContainsKey(anything.GetType()))
                return converters[anything.GetType()].Invoke(this,
                    new[] {anything}) as IDictionary<string, object>;
            return WithCommonConventions(anything);
        }

        public IDictionary<Type, MethodInfo> ToDictionaryMethods()
        {
            return this.GetType().GetTypeInfo().GetMethods()
                .Where(x => x.Name == "ToDictionary")
                .Where(x => x.GetParameters().Length == 1)
                .Select(x => new
                {
                    TheType = x.GetParameters().First().ParameterType,
                    TheMethod = x
                }).ToList()
                .ToDictionary(x => x.TheType, x => x.TheMethod);
        }

        private static bool AnyValuesWereSetOn(object target)
        {
            return target.GetType()
                .GetTypeInfo()
                .GetProperties()
                .Any(x => x.GetValue(target) != null);
        }

        private static IDictionary<string, object> RemoveNulls(IDictionary<string, object> dictionary)
        {
            var blanks = dictionary.Keys.Where(k => dictionary[k] == null).ToList();
            foreach (var key in blanks) dictionary.Remove(key);
            return dictionary;
        }

        private IDictionary<string, object> WithCommonConventions(object target, IDictionary<string, object> results = null)
        {
            if (target == null) return null;
            if (results == null) results = new Dictionary<string, object>();
            foreach (var property in target.GetType().GetTypeInfo().GetProperties())
            {
                var name = SnakeCase.Convert(property.Name);
                if (results.ContainsKey(name)) continue;

                results[name] = GetTheValue(property.PropertyType, property.GetValue(target));
            }
            return RemoveNulls(results);
        }

        public object GetTheValue(Type propertyType, object value)
        {
            var valueMapper = valueMappers.FirstOrDefault(x => x.CanMap(propertyType, value));
            return valueMapper == null ? value : valueMapper.Map(propertyType, value);
        }

        public IDictionary<string, object> ToDictionary(RecipientList recipientList)
        {

            var data = new Dictionary<string, object>
            {
                ["recipients"] = recipientList.Recipients.Select(ToDictionary),
                ["attributes"] = recipientList.Attributes != null 
                ? (object)new Dictionary<string, object> {
                                              ["internal_id"] = recipientList.Attributes.InternalId
                                            , ["list_group_id"] = recipientList.Attributes.ListGroupId } : null
            };
            var result = WithCommonConventions(recipientList, data);
            return result;

        }
    }
}