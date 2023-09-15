using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using NccCore.BackgroundJob;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace NccCore.BackgroundJob
{
    public class EmailBackgroundJob : BackgroundJob<EmailBackgroundJobArgs>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        public EmailBackgroundJob(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        [UnitOfWork]
        public override void Execute(EmailBackgroundJobArgs args)
        {
            Logger.Info("Email backgroung trigger!");
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.Body = args.Body;
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = args.Subject;
                Queue<string> queueEmail = new Queue<string>(args.TargetEmails);
                if (queueEmail.Count == 1)
                {
                    mailMessage.To.Add(new MailAddress(queueEmail.Peek()));
                    queueEmail.Dequeue();
                }
                while (queueEmail.Count > 0)
                {
                    if (queueEmail.Count == 1)
                    {
                        mailMessage.CC.Add(new MailAddress(queueEmail.Peek()));
                    }
                    else
                    {
                        mailMessage.To.Add(new MailAddress(queueEmail.Peek()));
                    }
                    queueEmail.Dequeue();
                }

                _emailSender.Send(
                        mailMessage, true
                    );
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}
