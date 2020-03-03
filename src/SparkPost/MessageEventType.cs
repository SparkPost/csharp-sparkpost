namespace SparkPost
{
    // Values taken from:
    // https://developers.sparkpost.com/api/events/
    // Additional values and descriptions taken from:
    // https://support.sparkpost.com/customer/portal/articles/1976204-webhook-event-reference

    public enum MessageEventType
    {
        /// <summary>
        /// Undefined/unknown or unable to parse.
        /// </summary>
        Undefined,

        /// <summary>
        /// delivery
        /// Delivery.
        /// Remote MTA acknowledged receipt of a message.
        /// </summary>
        Delivery,

        /// <summary>
        /// injection
        /// Injection.
        /// Message is received by or injected into SparkPost.
        /// </summary>
        Injection,

        /// <summary>
        /// bounce
        /// Bounce.
        /// Remote MTA has permanently rejected a message.
        /// </summary>
        Bounce,

        /// <summary>
        /// delay
        /// Delay.
        /// Remote MTA has temporarily rejected a message.
        /// </summary>
        Delay,

        /// <summary>
        /// policy_rejection
        /// Policy Rejection.
        /// Due to policy, SparkPost rejected a message or failed to generate a message.
        /// </summary>
        PolicyRejection,

        /// <summary>
        /// out_of_band
        /// Out of Band.
        /// Remote MTA initially reported acceptance of a message, but it has since asynchronously reported that the message was not delivered.
        /// </summary>
        OutOfBand,

        /// <summary>
        /// open
        /// Open.
        /// Recipient opened a message in a mail client, thus rendering a tracking pixel.
        /// </summary>
        Open,

        /// <summary>
        /// initial_open
        /// Initial Open.
        /// Recipient opened a message in a mail client, thus rendering a tracking pixel at the top of the message.
        /// </summary>
        InitialOpen,

        /// <summary>
        /// amp_click
        /// AMP Click.
        /// Recipient clicked a tracked link in an AMP message, thus prompting a redirect through the SparkPost click-tracking server to the link's destination.
        /// </summary>
        AmpClick,

        /// <summary>
        /// amp_open
        /// AMP Open.
        /// Recipient opened an AMP message in a mail client, thus rendering a tracking pixel at the bottom of the message.
        /// </summary>
        AmpOpen,

        /// <summary>
        /// amp_initial_open
        /// AMP Initial Open.
        /// Recipient opened an AMP message in a mail client, thus rendering a tracking pixel at the top of the message.
        /// </summary>
        AmpInitialOpen,

        /// <summary>
        /// click
        /// Click.
        /// Recipient clicked a tracked link in a message, thus prompting a redirect through the SparkPost click-tracking server to the link's destination.
        /// </summary>
        Click,

        /// <summary>
        /// generation_failure
        /// Generation Failure.
        /// Message generation failed for an intended recipient.
        /// </summary>
        GenerationFailure,

        /// <summary>
        /// generation_rejection
        /// Generation Rejection.
        /// SparkPost rejected message generation due to policy.
        /// </summary>
        GenerationRejection,

        /// <summary>
        /// spam_complaint
        /// Spam Complaint.
        /// Message was classified as spam by the recipient.
        /// </summary>
        SpamComplaint,

        /// <summary>
        /// list_unsubscribe
        /// List Unsubscribe.
        /// User clicked the 'unsubscribe' button on an email client.
        /// </summary>
        ListUnsubscribe,

        /// <summary>
        /// link_unsubscribe
        /// Link Unsubscribe.
        /// User clicked a hyperlink in a received email.
        /// </summary>
        LinkUnsubscribe,

        /// <summary>
        /// sms_status
        /// SMS Status.
        /// SMPP/SMS message produced a status log output.
        /// </summary>
        SmsStatus,

        /// <summary>
        /// relay_injection
        /// Relay Injection.
        /// Relayed message is received by or injected into SparkPost.
        /// </summary>
        RelayInjection,

        /// <summary>
        /// relay_rejection
        /// Relay Rejection.
        /// SparkPost rejected a relayed message or failed to generate a relayed message.
        /// </summary>
        RelayRejection,

        /// <summary>
        /// relay_delivery
        /// Relay Delivery.
        /// Remote HTTP Endpoint acknowledged receipt of a relayed message.
        /// </summary>
        RelayDelivery,

        /// <summary>
        /// relay_tempfail
        /// Relay Temporary Failure.
        /// Remote HTTP Endpoint has failed to accept a relayed message.
        /// </summary>
        RelayTempfail,

        /// <summary>
        /// relay_permfail
        /// Relay Permanent Failure.
        /// Relayed message has reached the maximum retry threshold and will be removed from the system.
        /// </summary>
        RelayPermfail

    }
}
