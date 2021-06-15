using System;
using DiscordBotHumEncore.Handlers;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace DiscordBotHumEncore
{
    class Program
    {
        private DiscordSocketClient client;
        private CommandService commands;
        private GlobalData globalData;

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
              //  LogLevel = LogSeverity.Critical
              //  LogLevel = LogSeverity.Debug
              //  LogLevel = LogSeverity.Error
              //  LogLevel = LogSeverity.Info
              //  LogLevel = LogSeverity.Verbose
              //  LogLevel = LogSeverity.Warning
            });

            commands = new CommandService();
            globalData = new GlobalData();
            
            client.Log += Log;
            client.Ready += () =>
            {
                Console.WriteLine(GlobalData.Config.ReadyLog);
                client.SetGameAsync(GlobalData.Config.Playing_status);
                return Task.CompletedTask;
            };

            await CommandAsync();
            await InitializeGlobalDataAsync();
            await client.LoginAsync(TokenType.Bot, GlobalData.Config.Token);
            await client.StartAsync();
            await Task.Delay(-1);
        }

        public async Task CommandAsync()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = (SocketUserMessage)msg;
            if (message == null) return;
            int argPos = 0;
            if (!message.HasStringPrefix(GlobalData.Config.Prefixe, ref argPos)) return;
            var context = new SocketCommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, null);
            if (!result.IsSuccess) 
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }
        private async Task InitializeGlobalDataAsync()
        {
            await globalData.InitializeAsync();
        }
    }
}
