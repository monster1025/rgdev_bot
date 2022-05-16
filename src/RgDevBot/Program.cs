using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RgDevBot.Config;

namespace RgDevBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new SentConfig(LogManager.GetCurrentClassLogger());
            var bot = new TelegramBot();
            var parser = new NewsParser(bot, config);
            while (true)
            {
                try
                {
                    await parser.Parse();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }
    }
}
