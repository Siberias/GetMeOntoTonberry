using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace GetMeOntoTonberry
{
    class Program
    {
        private static string serverName = "Tonberry";

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

                if(worldNameItems[0].InnerHtml.Contains(serverName))
                {
                    var characterCreationStatusItems = item.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("world-list__create_character")).ToList();

                    if(characterCreationStatusItems[0].InnerHtml.Contains("unavailable"))
                    {
                        Console.WriteLine(serverName + " is unavailable :(");
                    }
                    else
                    {
                        Console.WriteLine(serverName + " is available!!! GO GO GO!!!");
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
            MailMessage email = new MailMessage();

            email.From = new MailAddress("messengermoogle@gmail.com");
            email.To.Add("siberias.samt@gmail.com");
            email.Body = body;

            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

            smtpServer.Port = 587;
            smtpServer.Credentials = LoadEmailCredentialsFromFile();
            smtpServer.EnableSsl = true;

            smtpServer.Send(email);
        }

        class EmailCredentials
        {
           public string Username = "";
           public string Password = "";
        }

        private static System.Net.NetworkCredential LoadEmailCredentialsFromFile()
        {
            string fileName = "emailCredentials.json";

            string fileText = File.ReadAllText(fileName);

            EmailCredentials credentialsFromFile = JsonConvert.DeserializeObject<EmailCredentials>(fileText);

            return new System.Net.NetworkCredential(credentialsFromFile.Username, credentialsFromFile.Password);
        }
    }
}
