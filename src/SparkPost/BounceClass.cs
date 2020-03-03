namespace SparkPost
{
    // The list was taken from:
    // https://support.sparkpost.com/customer/portal/articles/1929896

    public enum BounceClass
    {
        /// <summary>
        /// None or unable to parse.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Undetermined
        /// The response text could not be identified.
        /// Category: Undetermined.
        /// </summary>
        Undetermined = 1,

        /// <summary>
        /// Invalid Recipient
        /// The recipient is invalid.
        /// Category: Hard.
        /// </summary>
        InvalidRecipient = 10,

        /// <summary>
        /// Soft Bounce
        /// The message soft bounced.
        /// Category: Soft.
        /// </summary>
        SoftBounce = 20,

        /// <summary>
        /// DNS Failure
        /// The message bounced due to a DNS failure.
        /// Category: Soft.
        /// </summary>
        DnsFailure = 21,

        /// <summary>
        /// Mailbox Full
        /// The message bounced due to the remote mailbox being over quota.
        /// Category: Soft.
        /// </summary>
        MailboxFull = 22,

        /// <summary>
        /// Too Large
        /// The message bounced because it was too large for the recipient.
        /// Category: Soft.
        /// </summary>
        TooLarge = 23,

        /// <summary>
        /// Timeout
        /// The message timed out.
        /// Category: Soft.
        /// </summary>
        Timeout = 24,

        /// <summary>
        /// Admin Failure
        /// The message was failed by Momentum's configured policies.
        /// Category: Admin.
        /// </summary>
        AdminFailure = 25,

        /// <summary>
        /// Smart Send Suppression
        /// The message was suppressed by Smart Send policy.
        /// Category: Admin.
        /// </summary>
        SmartSendSuppression = 26,

        /// <summary>
        /// Generic Bounce: No RCPT
        /// No recipient could be determined for the message.
        /// Category: Hard.
        /// </summary>
        GenericBounceNoRecipient = 30,

        /// <summary>
        /// Generic Bounce
        /// The message failed for unspecified reasons.
        /// Category: Soft.
        /// </summary>
        GenericBounce = 40,

        /// <summary>
        /// Mail Block
        /// The message was blocked by the receiver.
        /// Category: Block.
        /// </summary>
        MailBlock = 50,

        /// <summary>
        /// Spam Block
        /// The message was blocked by the receiver as coming from a known spam source.
        /// Category: Block.
        /// </summary>
        SpamBlock = 51,

        /// <summary>
        /// Spam Content
        /// The message was blocked by the receiver as spam.
        /// Category: Block.
        /// </summary>
        SpamContent = 52,

        /// <summary>
        /// Prohibited Attachment
        /// The message was blocked by the receiver because it contained an attachment.
        /// Category: Block.
        /// </summary>
        ProhibitedAttachment = 53,

        /// <summary>
        /// Relaying Denied
        /// The message was blocked by the receiver because relaying is not allowed.
        /// Category: Block.
        /// </summary>
        RelayingDenied = 54,

        /// <summary>
        /// Auto-Reply
        /// The message is an auto-reply/vacation mail.
        /// Category: Soft.
        /// </summary>
        AutoReply = 60,

        /// <summary>
        /// Transient Failure
        /// Message transmission has been temporarily delayed.
        /// Category: Soft.
        /// </summary>
        TransientFailure = 70,

        /// <summary>
        /// Subscribe
        /// The message is a subscribe request.
        /// Category: Admin.
        /// </summary>
        Subscribe = 80,

        /// <summary>
        /// Unsubscribe
        /// The message is an unsubscribe request.
        /// Category: Hard.
        /// </summary>
        Unsubscribe = 90,

        /// <summary>
        /// Challenge-Response
        /// The message is a challenge-response probe.
        /// Category: Soft.
        /// </summary>
        ChallengeResponse = 100
    }
}
