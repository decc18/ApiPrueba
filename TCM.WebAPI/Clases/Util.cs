using Newtonsoft.Json.Linq;
using PushSharp.Core;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using TCM.Core.Entidades;

namespace TCM.WebAPI.Clases
{
    public class Util
    {
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        public static async Task EnviarMail(string para, string asunto, string cuerpo)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(para));
            message.From = new MailAddress(WebConfigurationManager.AppSettings["AdminUser"]);
            message.Subject = asunto;
            message.Body = cuerpo;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = WebConfigurationManager.AppSettings["AdminUser"],
                    Password = WebConfigurationManager.AppSettings["AdminPassWord"]
                };

                smtp.Credentials = credential;
                smtp.Host = WebConfigurationManager.AppSettings["SMTPName"];
                smtp.Port = int.Parse(WebConfigurationManager.AppSettings["SMTPPort"]);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }
        }

        public static string PopulateBody(string userName, string password)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/TemplateEmail.html")))
            {
                body = reader.ReadToEnd();
                body = body.Replace("{Usuario}", userName);
                body = body.Replace("{Password}", password);
            }
            return body;
        }

        public static void EnviarNotificacionPushFCM(List<DtoDispositivo> dispositivos, string titulo, string mensaje)
        {
            // Configuration GCM (use this section for GCM)
            var config = new GcmConfiguration("AIzaSyCiLge3SyEbl29Qo0B5fLriG1JWrnELANY", "AAAA5bNhwuU:APA91bExp8oylCzDRjFq5qN0OpmNjslf4m_R3jbIWNN_j9kbhCdpCx_tUXnmiRc6GnwbCAi7z5Q2ougftKfuawSZSUfEdImHVBvfdtmmJAK3ykix7CS6wjxS3DbwvbHzKTIDGInmT_RC", null);
            
            // Configuration FCM (use this section for FCM)
            // var config = new GcmConfiguration("APIKEY");
            // config.GcmUrl = "https://fcm.googleapis.com/fcm/send";
            // var provider = "FCM";

            // Create a new broker
            var gcmBroker = new GcmServiceBroker(config);

            gcmBroker.OnNotificationSucceeded += (notification) => {
                Console.WriteLine("{provider} Notification Sent!");
            };

            // Start the broker
            gcmBroker.Start();

            foreach (var regId in dispositivos)
            {
                // Queue a notification to send
                gcmBroker.QueueNotification(new GcmNotification
                {
                    RegistrationIds = new List<string> { regId.TokenPush },
                    Notification = JObject.Parse("{ \"body\":\""+ mensaje + "\",\"title\": \""+ titulo + "\",\"sound\": \"default\",\"silent\":false,\"content_available\" : true }"),
                    Data = JObject.Parse("{ \"message\" : \"my_custom_value\",\"other_key\" : true,\"body\":\"test\" }")
                });
            }

            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            gcmBroker.Stop();
        }

    }

    
}