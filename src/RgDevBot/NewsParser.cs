using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RgDevBot.Config;
using RgDevBot.ObjectModel;

namespace RgDevBot
{
    public class NewsParser
    {
        private readonly TelegramBot _bot;
        private readonly SentConfig _config;
        private int _lastSentId = -1;

        public NewsParser(TelegramBot bot, SentConfig config)
        {
            _bot = bot;
            _config = config;
        }

        public async Task Parse()
        {
            var url = "https://rg-dev.ru/api/news/?type=news";
            var content = await Get(url);

            var news = JsonConvert.DeserializeObject<NewsListResponse>(content);
            var latestNews = news?.results;

            Console.WriteLine($"[{DateTime.Now}] Получено {latestNews?.Count} новостей. Самая новая: {latestNews?.FirstOrDefault()?.id}.");

            foreach (var post in latestNews ?? new List<News>())
            {
                if (_config.ConfigValues.Contains(post.id))
                {
                    continue;
                }

                try
                {
                    var text = $"{post.title}:\r\nhttps://rg-dev.ru/press/news/all/{post.id}/";
                    Console.WriteLine(text);
                    _bot.SendMessage(text);

                    _config.ConfigValues.Add(post.id);
                    _config.Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public async Task<string> Get(string url, string contentType = "application/json")
        {
            var client = GetClientWithProxy(new Uri("http://192.168.1.6:9150"));
            var result = await client.GetAsync(url);
            return await result.Content.ReadAsStringAsync();
        }

        private HttpClient GetClientWithProxy(Uri proxy)
        {
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy($"socks5://{proxy.Host}:{proxy.Port}"),
                CheckCertificateRevocationList = false,
                MaxConnectionsPerServer = int.MaxValue,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                UseCookies = true,
                UseProxy = true,
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30),
            };
            return client;
        }
    }
}
