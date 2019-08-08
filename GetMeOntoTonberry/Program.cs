using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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
            QueryServerStatus();
            Console.ReadKey();
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
                    }
                }
            }
        }
    }
}
