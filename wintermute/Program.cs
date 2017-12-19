using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using Newtonsoft.Json;

namespace wintermute
{
    class Program
    {
        
        public static CommandsNextModule Commands { get; set; }

        static void Main(string[] args)
        {
            Run().GetAwaiter().GetResult();
        }

        public static async Task Run() {

            var discord = new DiscordClient(new DiscordConfig
            {
                AutoReconnect = true,
                DiscordBranch = Branch.Stable,
                LargeThreshold = 250,
                LogLevel = LogLevel.Debug,
                Token = "// Token goes here",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = false
            });
           
            // up next, let's set up our commands
            var ccfg = new CommandsNextConfiguration
            {
                // let's use the string prefix defined in config.json
                StringPrefix = "!",

                // enable responding in direct messages
                EnableDms = true,

                // enable mentioning the bot as a command prefix
                EnableMentionPrefix = true
            };

            // and hook them up
            Commands = discord.UseCommandsNext(ccfg);

            Commands.RegisterCommands<testcommands.TestCommands>();

            discord.DebugLogger.LogMessageReceived += (o, e) =>
            {
                Console.WriteLine($"[{e.Timestamp}] [{e.Application}] [{e.Level}] {e.Message}");
            };

            discord.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower() == "ping")
                    await e.Message.RespondAsync("pong");

            };

            discord.MessageReactionAdd += async e =>
            {
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord,":thinking:"));
            };

            discord.Ready += async e =>
            {
                await Task.Yield(); 
                discord.DebugLogger.LogMessage(LogLevel.Info, "Bot", "Ready! Setting status message..", DateTime.Now);
                // Sets Playing Status
                await discord.UpdateStatusAsync(new Game("is Online!"));
            };


            await discord.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}
