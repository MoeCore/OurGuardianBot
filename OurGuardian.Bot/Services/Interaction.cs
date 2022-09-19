using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Bot.Services;

public class Interaction
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly InteractionService _interactionService;
    private readonly ILogger _logger;
    private readonly IConfigurationRoot _configuration;

    public Interaction(
        DiscordSocketClient client,
        IServiceProvider serviceProvider,
        InteractionService interactionService,
        ILogger<Interaction> logger,
        IConfigurationRoot configuration)
    {
        _client = client;
        _serviceProvider = serviceProvider;
        _interactionService = interactionService;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task Init()
    {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        _client.Ready += OnClientReady;
        _client.InteractionCreated += HandleInteraction;
        _interactionService.SlashCommandExecuted += HandleSlashCommand;
    }

    private Task HandleSlashCommand(
        SlashCommandInfo slashCommand,
        IInteractionContext interactionContext,
        IResult result) => Task.CompletedTask;

    private async Task OnClientReady()
    {
        await Task.Delay(1000);
#if DEBUG
        await _interactionService.RegisterCommandsToGuildAsync(ulong.Parse(_configuration["Guilds:Test"]), true);
#else
        await _interactionService.RegisterCommandsGloballyAsync(true);
#endif
    }

    private async Task HandleInteraction(SocketInteraction socketInteraction)
    {
        try
        {
            var context = new SocketInteractionContext(_client, socketInteraction);
            await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Interaction handler");

            if (socketInteraction.Type == InteractionType.ApplicationCommand)
                await socketInteraction.GetOriginalResponseAsync().ContinueWith(async (message) => await message.Result.DeleteAsync());
        }
    }
}