using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Mail;

namespace GetMeOntoTonberry
{
    class Program
    {
        private static string ServerName = "Tonberry";

        static void Main(string[] args)
        {
            QueryServerStatus().GetAwaiter().GetResult();
        }

        private static async Task QueryServerStatus()
        {
            string url = "https://jp.finalfantasyxiv.com/lodestone/worldstatus/";
            var httpClient = new HttpClient();
            string html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var worldListItems = htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("world-list__item")).ToList();

            foreach(var item in worldListItems)
            {
                var worldNameItems = item.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("world-list__world_name")).ToList();

                if(worldNameItems[0].InnerHtml.Contains(ServerName))
                {
                    var characterCreationStatusItems = item.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("world-list__create_character")).ToList();

                    if(characterCreationStatusItems[0].InnerHtml.Contains("unavailable"))
                    {
                        Console.WriteLine(ServerName + " is unavailable :(");
                    }
                    else
                    {
                        Console.WriteLine(ServerName + " is available!!! GO GO GO!!!");
                        SendSuccessEmail();
                    }

                    return;
                }
            }

            Console.WriteLine("We couldn't find Tonberry :(");
        }

        private static void SendTestEmail()
        {
            SendEmail
                (
                    "Hello Adventurer! Kupo kupo!\n" +
                    "\n" +
                    "From your friend, the Messenger Moogle"
                );
        }

        private static void SendSuccessEmail()
        {
            SendEmail
                 (
                    "Tonberry is available kupo!!\n" +
                    "\n" +
                    "From your friend, the Messenger Moogle"
                 );
        }

        private static void SendEmail(string body)
        {
            EmailConfiguration emailConfiguration = LoadEmailCredentialsFromFile();

            MailMessage email = new MailMessage();

            email.From = new MailAddress(emailConfiguration.Sender.Email);
            email.To.Add(emailConfiguration.Receiver.Email);
            email.Body = body;

            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential(emailConfiguration.Sender.Email, emailConfiguration.Sender.Password);
            smtpServer.EnableSsl = true;

            smtpServer.Send(email);
        }

        class EmailConfiguration
        {
            public SenderDetails Sender;
            public ReceiverDetails Receiver;

            public class SenderDetails
            {
                public string Email = "";
                public string Password = "";
            }

            public class ReceiverDetails
            {
                public string Email = "";
            }
        }

        private static EmailConfiguration LoadEmailCredentialsFromFile()
        {
            string fileName = "EmailConfiguration.json";

            string fileText = File.ReadAllText(fileName);

            EmailConfiguration credentialsFromFile = JsonConvert.DeserializeObject<EmailConfiguration>(fileText);

            return credentialsFromFile;
        }
    }
}
