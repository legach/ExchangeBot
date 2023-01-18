using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ExchangeBot
{
    public class MessageHandler
    {
        private readonly DataRetriever _dataRetriever;
        private Dictionary<string, (DateTime timestamp, object value)> _cache;
        private TimeSpan _cachePeriod;


        public MessageHandler(DataRetriever dataRetriever)
        {
            _cache = new Dictionary<string, (DateTime, object)>();
            _dataRetriever = dataRetriever;
            _cachePeriod = TimeSpan.FromHours(3);
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

            var responseText = "Sorry, I don't understand you :\\(";
            try
            {
                switch (messageText)
                {
                    case "/getAll":
                        responseText = "*Buy/Sale*\n";
                        responseText += string.Join("\n", GetAllRates().Select(r => r));
                        break;
                    case "/getEuroAll":
                        responseText = "*Buy/Sale*\n";
                        responseText += string.Join("\n", GetAllRates().Select(r => r.ToString(Currency.Euro)));
                        break;
                    case "/getUsdAll":
                        responseText = "*Buy/Sale*\n";
                        responseText += string.Join("\n", GetAllRates().Select(r => r.ToString(Currency.Usd)));
                        break;
                    case "/getEuroDeal":
                        responseText = $"Minimum sale: \n {GetMinimumSalesRate(Currency.Euro)?.ToString(Currency.Euro) ?? "Unknown"} ";
                        responseText += $"\nMaximum buy: \n {GetMaximumBuysRate(Currency.Euro)?.ToString(Currency.Euro) ?? "Unknown"}";
                        break;
                    case "/getUsdDeal":
                        responseText = $"Minimum sale: \n {GetMinimumSalesRate(Currency.Usd)?.ToString(Currency.Usd) ?? "Unknown"} ";
                        responseText += $"\nMaximum buy: \n {GetMaximumBuysRate(Currency.Usd)?.ToString(Currency.Usd) ?? "Unknown"}";
                        break;
                }
            }
            catch (Exception e)
            {
                responseText = "Sorry, I have an internal problem :\\(";
            }
            
            
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                parseMode:ParseMode.MarkdownV2,
                text: responseText,
                disableWebPagePreview:true,
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

        private List<Rate> GetAllRates()
        {
            var key = nameof(GetAllRates);
            var result = new List<Rate>();
            if (_cache.ContainsKey(key) && ((DateTime.Now - _cache[key].timestamp) <= _cachePeriod))
            {
                result = _cache[key].value as List<Rate>;
            }

            if (result != null && result.Any()) 
                return result;

            result = _dataRetriever.GetAllRates();
            _cache.Add(key,(DateTime.Now, result));

            return result;
        }

        private Rate? GetMinimumSalesRate(Currency currency)
        {
            var key = nameof(GetMinimumSalesRate) + currency;
            Rate? result = null;
            if (_cache.ContainsKey(key) && ((DateTime.Now - _cache[key].timestamp) <= _cachePeriod))
            {
                result = _cache[key].value as Rate;
            }

            if (result != null) 
                return result;
            
            result = _dataRetriever.GetMinimumSalesRate(currency);
            if (result != null)
                _cache.Add(key,(DateTime.Now, result));

            return result;
        }

        private Rate? GetMaximumBuysRate(Currency currency)
        {
            var key = nameof(GetMaximumBuysRate) + currency;
            Rate? result = null;
            if (_cache.ContainsKey(key) && ((DateTime.Now - _cache[key].timestamp) <= _cachePeriod))
            {
                result = _cache[key].value as Rate;
            }

            if (result != null)
                return result;

            result = _dataRetriever.GetMaximumBuysRate(currency);
            if (result != null)
                _cache.Add(key, (DateTime.Now, result));

            return result;
        }
    }
}
