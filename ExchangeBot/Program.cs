using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ExchangeBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("TgApiKey");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("Cant launch with empty api-key. Please check <TgApiKey> environment variable");
                Console.ReadLine();
                return;
            }

            var botClient = new TelegramBotClient(apiKey);

            using CancellationTokenSource cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            var dataRetriever = new DataRetriever();
            var messageHandler = new MessageHandler(dataRetriever);

            botClient.StartReceiving(
                updateHandler: messageHandler.HandleUpdateAsync,
                pollingErrorHandler: messageHandler.HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync(cancellationToken: cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }
    }
}
