﻿using CMS.Common;
using CMS.Web.Logger;
using CMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Threading;

namespace CMS.Web.Helpers
{
    public class EmailService : IEmailService
    {
        readonly ILogger _logger;
        public EmailService(ILogger logger)
        {
            _logger = logger;
        }

        public bool Send(MailModel model)
        {

            try
            {
                if (string.IsNullOrEmpty(model.From))

                model.From = ConfigurationManager.AppSettings[Constants.FromEmail];

                MailMessage MyMailMessage = new MailMessage();
                MyMailMessage.Subject = model.Subject;
                MyMailMessage.Body = model.Body;
                if (model.To != null)
                    MyMailMessage.To.Add(model.To);
                if (model.IsBranchAdmin)
                    MyMailMessage.To.Add(ConfigurationManager.AppSettings[Common.Constants.AdminEmail]);
                if (model.IsClientAdmin)
                    MyMailMessage.To.Add(ConfigurationManager.AppSettings[Common.Constants.AdminEmail]);
                MyMailMessage.From = new MailAddress(model.From);
                MyMailMessage.IsBodyHtml = true;
                System.Net.NetworkCredential mailAuthentication = new System.Net.NetworkCredential(ConfigurationManager.AppSettings[Constants.FromEmail], ConfigurationManager.AppSettings[Constants.EmailPassword]);
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings[Constants.SmtpServer]; //SmtpServer
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.SmtpPort]); //Port
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = mailAuthentication;
                /*foreach (var path in model.AttachmentPaths)
                {
                    MyMailMessage.Attachments.Add(new Attachment(path) { Name = Path.GetFileName(path) });
                }*/
                //await smtp.SendMailAsync(MyMailMessage);
                smtp.Send(MyMailMessage);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
                _logger.Error(ex.Message + "email one user from" + model.From + " to" + model.To);
                return false;
            }
            return true;
        }

        public bool Send(MailModel model, List<string> multipleRecipients)
        {
            try
            {
                if (string.IsNullOrEmpty(model.From))
                    model.From = ConfigurationManager.AppSettings[Constants.FromEmail];

                MailMessage MyMailMessage = new MailMessage();
                MyMailMessage.Subject = model.Subject;
                MyMailMessage.Body = model.Body;

                multipleRecipients.ForEach(x =>
                {
                    MyMailMessage.To.Add(x);
                });

                MyMailMessage.From = new MailAddress(model.From);
                MyMailMessage.IsBodyHtml = true;
                System.Net.NetworkCredential mailAuthentication = new System.Net.NetworkCredential(ConfigurationManager.AppSettings[Constants.FromEmail], ConfigurationManager.AppSettings[Constants.EmailPassword]);
                SmtpClient smtp = new SmtpClient("relay-hosting.secureserver.net", 587);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = mailAuthentication;
                /*foreach (var path in model.AttachmentPaths)
                {
                    MyMailMessage.Attachments.Add(new Attachment(path) { Name = Path.GetFileName(path) });
                }*/
                smtp.Send(MyMailMessage);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + "email multiple");
                return false;
            }
            return true;
        }

        public void StartProcessing(MailModel[] mailModels, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                {
                    foreach (var mailModel in mailModels)
                    {
                        //execute when task has been cancel  
                        cancellationToken.ThrowIfCancellationRequested();
                        //send email here
                        Send(mailModel);
                        _logger.Info("Email Send Successfully");
                        Thread.Sleep(1500);   // wait to 1.5 sec every time  
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Occured : " + ex.GetType().ToString() + " : " + ex.Message);
            }
        }
    }
}