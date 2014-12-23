﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Kore.Data;
using Kore.Data.Services;
using Kore.Web.ContentManagement.Messaging.Domain;

namespace Kore.Web.ContentManagement.Messaging.Services
{
    public interface IMessageService : IGenericDataService<QueuedEmail>
    {
        Guid SendEmailMessage(string messageTemplate, IEnumerable<Token> tokens, string toEmailAddress, string toName = null);

        Guid SendEmailMessage(string subject, string body, string toEmailAddress, string toName = null);

        Guid SendEmailMessage(MailMessage mailMessage);
    }

    public class MessageService : GenericDataService<QueuedEmail>, IMessageService, IQueuedMessageProvider
    {
        private readonly IMessageTemplateService messageTemplateService;
        private readonly ITokenizer tokenizer;
        private readonly IEnumerable<IMessageTokensProvider> tokenProviders;

        public MessageService(
            IMessageTemplateService messageTemplateService,
            IRepository<QueuedEmail> queuedEmailRepository,
            ITokenizer tokenizer,
            IEnumerable<IMessageTokensProvider> tokenProviders)
            : base(queuedEmailRepository)
        {
            this.tokenizer = tokenizer;
            this.tokenProviders = tokenProviders;
            this.messageTemplateService = messageTemplateService;
        }

        public Guid SendEmailMessage(string messageTemplate, IEnumerable<Token> tokens, string toEmailAddress, string toName = null)
        {
            var template = messageTemplateService.GetTemplate(messageTemplate);
            if (template == null || !template.Enabled)
            {
                return Guid.Empty;
            }

            foreach (var tokenProvider in tokenProviders)
            {
                tokenProvider.GetTokens(messageTemplate, tokens);
            }

            return SendMessage(template, tokens, toEmailAddress, toName);
        }

        public Guid SendEmailMessage(string subject, string body, string toEmailAddress, string toName = null)
        {
            var mailMessage = new MailMessage
            {
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(new MailAddress(toEmailAddress, toName));

            return SendEmailMessage(mailMessage);
        }

        public Guid SendEmailMessage(MailMessage mailMessage)
        {
            var mailMessageWrap = new MailMessageWrapper(mailMessage);

            var queuedEmail = new QueuedEmail
            {
                Id = Guid.NewGuid(),
                Priority = 5,
                ToAddress = mailMessage.To[0].Address,
                ToName = mailMessage.To[0].DisplayName,
                Subject = mailMessage.Subject,
                MailMessage = mailMessageWrap.ToString(),
                CreatedOnUtc = DateTime.UtcNow
            };

            Insert(queuedEmail);

            return queuedEmail.Id;
        }

        private Guid SendMessage(Domain.MessageTemplate messageTemplate,
            IEnumerable<Token> tokens, string toEmailAddress, string toName)
        {
            var subject = messageTemplate.Subject ?? string.Empty;
            var body = messageTemplate.Body ?? string.Empty;

            //Replace subject and body tokens
            var subjectReplaced = tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = tokenizer.Replace(body, tokens, true);

            return SendEmailMessage(subjectReplaced, bodyReplaced, toEmailAddress, toName);
        }

        public IEnumerable<IMailMessage> GetQueuedEmails(int maxSendTries, int maxMessageItems)
        {
            return Repository.Table
                .Where(x => x.SentTries < maxSendTries && x.SentOnUtc == null)
                .OrderBy(x => x.Priority).ThenBy(x => x.CreatedOnUtc)
                .Take(maxMessageItems)
                .ToList();
        }

        public void OnSendSuccess(IMailMessage mailMessage)
        {
            var entity = (QueuedEmail)mailMessage;
            entity.SentOnUtc = DateTime.UtcNow;
            Repository.Update(entity);
        }

        public void OnSendError(IMailMessage mailMessage)
        {
            var entity = (QueuedEmail)mailMessage;
            entity.SentTries++;
            Repository.Update(entity);
        }
    }
}