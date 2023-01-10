using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lalraen.AzureFunctions.Shared
{
    public class TelegramBotService
    {
        private readonly ITelegramBotClient _telegramBotClient;

        public TelegramBotService(string botToken)
        {
            _telegramBotClient = new TelegramBotClient(botToken);
        }

        public async Task SendMessageAsync(ChatId chatId, string message, bool disableNotification = false)
        {
            CheckMessageString(message);

            await SendTextMessageWithAction(chatId, message, disableNotification)
                .ConfigureAwait(false);
        }

        public async Task SendMessageAsync(ChatId chatId, string message, IReplyMarkup inline,
            bool disableNotification = false)
        {
            CheckMessageString(message);

            await SendTextMessageWithAction(chatId, message, inline, disableNotification)
                .ConfigureAwait(false);
        }

        public async Task SendMessageToAllClientsAsync(IEnumerable<ChatId> clientChatIds, string message,
            bool disableNotification = false)
        {
            CheckMessageString(message);

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

        private async Task SendTextMessageWithAction(ChatId chatId, string message, IReplyMarkup replyMarkup,
            bool disableNotification)
        {
            await _telegramBotClient.SendChatActionAsync(chatId, ChatAction.Typing)
                .ConfigureAwait(false);

            await _telegramBotClient.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup,
                    disableNotification: disableNotification)
                .ConfigureAwait(false);
        }

        private void CheckMessageString(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                var errorMessage = $"{nameof(message)} has no content. String is empty.";

                throw new ArgumentNullException(nameof(message), errorMessage);
            }
        }
    }
}
