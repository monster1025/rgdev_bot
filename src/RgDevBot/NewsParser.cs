using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using RgDevBot.ObjectModel;

namespace RgDevBot
{
    public class NewsParser
    {
        private readonly TelegramBot _bot;
        private int _lastSentId = -1;

        public NewsParser(TelegramBot bot)
        {
            _bot = bot;
        }

        public void Parse()
        {
            var url = "https://rg-dev.ru/api/news/?type=news";
            var content = Get(url);

            var news = JsonConvert.DeserializeObject<NewsListResponse>(content);
            var latestNews = news.results.OrderByDescending(f => f.id).FirstOrDefault();

            if (latestNews != null && latestNews.id != _lastSentId)
            {
                var text = $"{latestNews.title}:\r\nhttps://rg-dev.ru/press/news/all/{latestNews.id}/";
                Console.WriteLine(text);
                _bot.SendMessage(text);

                _lastSentId = latestNews.id;
            }
        }



        public string Get(string url, string contentType = "application/json")
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.ContentType = contentType;
            httpRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
    }
}
