using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lalraen.AzureFunctions.Shared
{
    public class TelegramBotService
    {
        private readonly ILogger _logger;
        private readonly ITelegramBotClient _telegramBotClient;

        public TelegramBotService(ILogger logger, string botToken)
        {
            _logger = logger;
            _telegramBotClient = new TelegramBotClient(botToken);
        }

        public async Task SendMessageAsync(ChatId chatId, string message)
        {
            CheckMessageString(message);

            _logger.LogInformation(message);

            await SendTextMessageWithAction(chatId, message);
        }

        public async Task SendMessageToAllClientsAsync(IEnumerable<ChatId> clientChatIds, string message)
        {
            CheckMessageString(message);

            _logger.LogInformation(message);

            foreach (var chatId in clientChatIds)
            {
                await SendTextMessageWithAction(chatId, message);
            }
        }

        private async Task SendTextMessageWithAction(ChatId chatId, string message)
        {
            await _telegramBotClient.SendChatActionAsync(chatId, ChatAction.Typing);
            await _telegramBotClient.SendTextMessageAsync(chatId, message);
        }

        private void CheckMessageString(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                const string ErrorMessage = $"{nameof(message)} has no content. String is empty.";

                _logger.LogError(ErrorMessage);

                throw new ArgumentNullException(nameof(message), ErrorMessage);
            }
        }
    }
}