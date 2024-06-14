using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace realEstateManagementAPI.UtilityHelper
{
    public class MailHelper
    {
        private readonly IConfiguration _configuration;
        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void MailSender(string Subject, string To, string Body, MailPriority mailPriority)
        {
            // Mail gönderimi
            MailMessage msg = new MailMessage();
            msg.Subject = Subject;
            msg.From = new MailAddress("aridurufakitayran@gmail.com", "ESRS Real Estate");
            
            
            if(To is null)
            {
                msg.To.Add("fakitayranariduru@hotmail.com");
            }
            else
            {
                msg.To.Add(To);
            }
            msg.Body = Body;
            msg.Priority = mailPriority;

            // SMTP istemcisini ayarlama
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            var smtpUsername = _configuration["AppSettings:NetworkCredantialsSettings:Username"];
            var smtpPassword = _configuration["AppSettings:NetworkCredantialsSettings:Password"];


            // Kimlik doğrulama bilgilerinin ayarlanması
            NetworkCredential credentials = new NetworkCredential(smtpUsername, smtpPassword);

            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            // Mesajın gönderilmesi
            client.Send(msg);
        }
    }
}
