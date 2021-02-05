using System;
using System.Threading;

namespace RgDevBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new TelegramBot();
            var parser = new NewsParser(bot);
            while (true)
            {
                parser.Parse();
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
