using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace ExchangeBot
{
    public class MessageHandler
    {
        private readonly DataRetriever _dataRetriever;

        public MessageHandler(DataRetriever dataRetriever)
        {
            _dataRetriever = dataRetriever;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            var responseText = "Sorry, I don't understand you :(";
            if (messageText == "/getAll")
            {
                try
                {
                    var rates = _dataRetriever.GetAllRates();
                    responseText = string.Join("\n", rates.Select(r => $"{r.Company.Name} Sale:{r.Sales[Currency.Euro]}"));
                }
                catch (Exception e)
                {
                    responseText = "Sorry, I have an internal problem :(";
                }
            } else if (messageText == "/getEuroDeal")
            {
                try
                {
                    var salesRate = _dataRetriever.GetMinimumSalesRate(Currency.Euro);
                    responseText = $"Minimum sale: \n {salesRate.Sales[Currency.Euro]} in {salesRate.Company.Name}";
                    var buyRate = _dataRetriever.GetMaximumBuysRate(Currency.Euro);
                    responseText += $"\nMaximum buy: \n {buyRate.Buys[Currency.Euro]} in {buyRate.Company.Name}";
                }
                catch (Exception e)
                {
                    responseText = "Sorry, I have an internal problem :(";
                }
            }
            
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: responseText,
                cancellationToken: cancellationToken);
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
