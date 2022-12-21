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

        public async Task SendMessageAsync(ChatId chatId, string message, bool disableNotification = false)
        {
            CheckMessageString(message);

            _logger.LogInformation(message);

            await SendTextMessageWithAction(chatId, message, disableNotification)
                .ConfigureAwait(false);
        }

        public async Task SendMessageToAllClientsAsync(IEnumerable<ChatId> clientChatIds, string message,
            bool disableNotification = false)
        {
            CheckMessageString(message);

            _logger.LogInformation(message);

            foreach (var chatId in clientChatIds)
            {
                await SendTextMessageWithAction(chatId, message, disableNotification)
                    .ConfigureAwait(false);
            }
        }

        private async Task SendTextMessageWithAction(ChatId chatId, string message, bool disableNotification)
        {
            await _telegramBotClient.SendChatActionAsync(chatId, ChatAction.Typing)
                .ConfigureAwait(false);

            await _telegramBotClient.SendTextMessageAsync(chatId, message, disableNotification: disableNotification)
                .ConfigureAwait(false);
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