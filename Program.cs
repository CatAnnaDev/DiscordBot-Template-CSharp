using System;
using DiscordBotHumEncore.Handlers;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using DiscordBotHumEncore.Services;

namespace DiscordBotHumEncore
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private GlobalData _globalData;

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                //  LogLevel = LogSeverity.Critical
                //  LogLevel = LogSeverity.Debug
                //  LogLevel = LogSeverity.Error
                //  LogLevel = LogSeverity.Info
                //  LogLevel = LogSeverity.Verbose
                //  LogLevel = LogSeverity.Warning
            });
            _commands = new CommandService();
            _globalData = new GlobalData();
            
            _client.Log += Log;
            _client.Ready += ReadyAsync;

            await CommandAsync();
            await InitializeGlobalDataAsync();
            await _client.LoginAsync(TokenType.Bot, GlobalData.Config.Token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        public async Task CommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = (SocketUserMessage)msg;
            if (message == null) return;
            int argPos = 0;
            if (!message.HasStringPrefix(GlobalData.Config.Prefixe, ref argPos)) return;
            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, null);
            if (!result.IsSuccess) 
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private async Task Log(LogMessage arg)
        {
            await LoggingService.LogAsync(arg.Source, arg.Severity, arg.Message);

        }
        private async Task ReadyAsync()
        {
            try
            {
                if (GlobalData.Config.ReadyLog.Length > 0)
                    Console.WriteLine(GlobalData.Config.ReadyLog);

                await _client.SetGameAsync(GlobalData.Config.Playing_status);
            }
            catch (Exception ex)
            {
                await LoggingService.LogInformationAsync(ex.Source, ex.Message);
            }
        }
        private async Task InitializeGlobalDataAsync()
        {
            await _globalData.InitializeAsync();
        }
    }
}
