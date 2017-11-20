using SparkPost.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SparkPost
{
    public class MessageEvent
    {
        /// <summary>
        /// "type": {
        ///   "description": "Type of event this record describes",
        ///   "sampleValue": "bounce"
        /// }
        /// "type": "out_of_band",
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Type of event this record describes
        /// </summary>
        public MessageEventType TypeEnum
        {
            get
            {
                foreach (var typeName in Enum.GetNames(typeof(MessageEventType)))
                {
                    var typeNameSnakeCase = SnakeCase.Convert(typeName);
                    if (string.Equals(Type, typeNameSnakeCase))
                        // check for an unmapped message event type here
                        return (MessageEventType)Enum.Parse(typeof(MessageEventType), typeName);
                }
                return MessageEventType.Undefined;
            }
        }

        /// <summary>
        /// "bounce_class": {
        ///   "description": "Classification code for a given message (see [Bounce Classification Codes](https://support.sparkpost.com/customer/portal/articles/1929896))",
        ///   "sampleValue": "1"
        /// },
        /// "bounce_class": "10",
        /// </summary>
        public string BounceClass { get; set; }

        /// <summary>
        /// Classification code for a given message (see [Bounce Classification Codes](https://support.sparkpost.com/customer/portal/articles/1929896))
        /// </summary>
        public BounceClass BounceClassEnum
        {
            get
            {
                int bounceClassAsInt;
                if (!int.TryParse(BounceClass, out bounceClassAsInt)) return SparkPost.BounceClass.Undefined;
                // note:  these scare me, perhaps we should check that it is valid?
                var bounceClass = (BounceClass)bounceClassAsInt;
                return bounceClass.ToString() == bounceClassAsInt.ToString()
                    ? SparkPost.BounceClass.Undefined
                    : bounceClass;
            }
        }

        /// <summary>
        /// Classification code for a given message (see [Bounce Classification Codes](https://support.sparkpost.com/customer/portal/articles/1929896))
        /// </summary>
        public BounceClassDetails BounceClassDetails => BounceClassesDetails.AllBounceClasses[BounceClassEnum];

        /// <summary>
        /// "campaign_id": {
        ///   "description": "Campaign of which this message was a part",
        ///   "sampleValue": "Example Campaign Name"
        /// },
        /// "campaign_id": "My campaign name",
        /// </summary>
        public string CampaignId { get; set; }

        /// <summary>
        /// "customer_id": {
        ///   "description": "SparkPost-customer identifier through which this message was sent",
        ///   "sampleValue": "1"
        /// },
        /// "customer_id": "12345",
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// "delv_method": {
        ///   "description": "Protocol by which SparkPost delivered this message",
        ///   "sampleValue": "esmtp"
        /// },
        /// "delv_method": "esmtp",
        /// </summary>
        public string DeliveryMethod { get; set; }

        /// <summary>
        /// "device_token": {
        ///   "description": "Token of the device / application targeted by this PUSH notification message. Applies only when delv_method is gcm or apn.",
        ///   "sampleValue": "45c19189783f867973f6e6a5cca60061ffe4fa77c547150563a1192fa9847f8a"
        /// },
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        /// "error_code": {
        ///   "description": "Error code by which the remote server described a failed delivery attempt",
        ///   "sampleValue": "554"
        /// },
        /// "error_code": "550",
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// "ip_address": {
        ///   "description": "IP address of the host to which SparkPost delivered this message; in engagement events, the IP address of the host where the HTTP request originated",
        ///   "sampleValue": "127.0.0.1"
        /// },
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// "message_id": {
        ///   "description": "SparkPost-cluster-wide unique identifier for this message",
        ///   "sampleValue": "0e0d94b7-9085-4e3c-ab30-e3f2cd9c273e"
        /// },
        /// "message_id": "00021f9a27476a273c57",
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// "msg_from": {
        ///   "description": "Sender address used on this message's SMTP envelope",
        ///   "sampleValue": "sender@example.com"
        /// },
        /// "msg_from": "msprvs1=17827RA6TC8Pz=bounces-12345@sparkpostmail1.com",
        /// </summary>
        public string MessageFrom { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use MessageEvent.MessageFrom.")]
        public string MessageForm { get { return MessageFrom; } set { MessageFrom = value; } }

        /// <summary>
        /// "msg_size": {
        ///   "description": "Message's size in bytes",
        ///   "sampleValue": "1337"
        /// },
        /// "msg_size": "3168",
        /// </summary>
        public string MessageSize { get; set; }

        /// <summary>
        /// "num_retries": {
        ///   "description": "Number of failed attempts before this message was successfully delivered; when the first attempt succeeds, zero",
        ///   "sampleValue": "2"
        /// },
        /// "num_retries": "0",
        /// </summary>
        public string NumberOfRetries { get; set; }

        /// <summary>
        /// "rcpt_meta": {
        ///   "description": "Metadata describing the message recipient",
        ///   "sampleValue": {
        ///     "customKey": "customValue"
        ///   }
        /// },
        ///"rcpt_meta": {
        ///  "CustomKey1": "Custom Value 1",
        ///  "CustomKey2": "Custom Value 2"
        ///},
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// "rcpt_tags": {
        ///   "description": "Tags applied to the message which generated this event",
        ///   "sampleValue": [
        ///     "male",
        ///     "US"
        ///   ]
        /// },
        /// "rcpt_tags": [ "CustomTag1" ],
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// "rcpt_to": {
        ///   "description": "Recipient address used on this message's SMTP envelope",
        ///   "sampleValue": "recipient@example.com"
        /// },
        /// "rcpt_to": "to@domain.com",
        /// </summary>
        public string RecipientTo { get; set; }

        /// <summary>
        /// "rcpt_type": {
        ///   "description": "Indicates that a recipient address appeared in the Cc or Bcc header or the archive JSON array",
        ///   "sampleValue": "cc"
        /// },
        /// </summary>
        public string RecipientType { get; set; }

        /// <summary>
        /// "raw_reason": {
        ///   "description": "Unmodified, exact response returned by the remote server due to a failed delivery attempt",
        ///   "sampleValue": "MAIL REFUSED - IP (17.99.99.99) is in black list"
        /// },
        /// "raw_reason": "550 [internal] [oob] The recipient is invalid.",
        /// </summary>
        public string RawReason { get; set; }

        /// <summary>
        /// "reason": {
        ///   "description": "Canonicalized text of the response returned by the remote server due to a failed delivery attempt",
        ///   "sampleValue": "MAIL REFUSED - IP (a.b.c.d) is in black list"
        /// },
        /// "reason": "550 [internal] [oob] The recipient is invalid.",
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// "routing_domain": {
        ///   "description": "Domain receiving this message",
        ///   "sampleValue": "example.com"
        /// },
        /// "routing_domain": "domain.com",
        /// </summary>
        public string RoutingDomain { get; set; }

        /// <summary>
        /// "subject": {
        ///   "description": "Subject line from the email header",
        ///   "sampleValue": "Summer deals are here!"
        /// },
        /// "subject": "My email subject",
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// "template_id": {
        ///   "description": "Slug of the template used to construct this message",
        ///   "sampleValue": "templ-1234"
        /// },
        /// "template_id": "smtp_47287131967131576",
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// "template_version": {
        ///   "description": "Version of the template used to construct this message",
        ///   "sampleValue": "1"
        /// },
        /// "template_version": "0",
        /// </summary>
        public string TemplateVersion { get; set; }

        /// <summary>
        /// "timestamp": {
        ///   "description": "Event date and time, in Unix timestamp format (integer seconds since 00:00:00 GMT 1970-01-01)",
        ///   "sampleValue": 1427736822
        /// },
        /// "timestamp": "2016-04-27T10:54:25.000+00:00"
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// "transmission_id": {
        ///   "description": "Transmission which originated this message",
        ///   "sampleValue": "64836157861974168"
        /// }
        /// "transmission_id": "47287131967131576",
        /// </summary>
        public string TransmissionId { get; set; }

        /// <summary>
        /// Not documented.
        /// "event_id": "84320004715329757",
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Not documented.
        /// "friendly_from": "from@domain.com",
        /// </summary>
        public string FriendlyFrom { get; set; }

        /// <summary>
        /// Not documented.
        /// "ip_pool": "shared",
        /// </summary>
        public string IpPool { get; set; }

        /// <summary>
        /// Not documented.
        /// "queue_time": "3004",
        /// </summary>
        public string QueueTime { get; set; }

        /// <summary>
        /// Not documented.
        /// "raw_rcpt_to": "to@domain.com",
        /// </summary>
        public string RawRecipientTo { get; set; }

        /// <summary>
        /// Not documented.
        /// "sending_ip": "shared",
        /// </summary>
        public string SendingIp { get; set; }

        /// <summary>
        /// Not documented.
        /// "tdate": "2016-04-27T22:05:40.000Z",
        /// </summary>
        public DateTime TDate { get; set; }

        /// <summary>
        /// Not documented.
        /// "transactional": "1",
        /// </summary>
        public string Transactional { get; set; }

        /// <summary>
        /// Not documented.
        /// "remote_addr": "A.B.C.D:25512",
        /// </summary>
        public string RemoteAddress { get; set; }

        public override string ToString()
        {
            return $"{TypeEnum} from {FriendlyFrom} to {RecipientTo}";
        }
    }
}