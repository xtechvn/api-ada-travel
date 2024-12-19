using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Utilities
{
    public class TelegramBotReply
    {
        private string telegram_group = "-759194623";
        private string telegram_token = "5321912147:AAFhcJ9DolwPWL74WbMjOOyP6-0G7w88PWY";
        
       
        private async Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            return;
        }

        private async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {
            if (update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                var text = update.Message.Text;
                var username = update.Message.From.Id;
                var id = update.Message.From.Username;
                await bot.SendTextMessageAsync(telegram_group, "ID: " + id + " . Username: " + username + " đã gửi message: " + text);
            }
        }
    }
}
