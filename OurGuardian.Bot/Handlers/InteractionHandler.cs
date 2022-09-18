using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace OurGuardian.Bot.Handlers;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;

    public InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
    {
        _client = client;
        _commands = commands;
        _services = services;
    }

    public async Task InitializeAsync()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.InteractionCreated += HandleInteraction;

        _commands.SlashCommandExecuted += SlashCommandExecuted;
        _commands.ContextCommandExecuted += ContextCommandExecuted;
        _commands.ComponentCommandExecuted += ComponentCommandExecuted;
    }

    private Task ComponentCommandExecuted(ComponentCommandInfo commandInfo, IInteractionContext interactionContext, IResult result)
    {
        return Task.CompletedTask;
    }

    private Task ContextCommandExecuted(ContextCommandInfo commandInfo, IInteractionContext interactionContext, IResult result)
    {
        return Task.CompletedTask;
    }

    private Task SlashCommandExecuted(SlashCommandInfo slashCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        return Task.CompletedTask;
    }

    private async Task HandleInteraction(SocketInteraction socketInteraction)
    {
        try
        {
            var context = new SocketInteractionContext(_client, socketInteraction);
            await _commands.ExecuteCommandAsync(context, _services);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);

            if (socketInteraction.Type == InteractionType.ApplicationCommand)
                await socketInteraction.GetOriginalResponseAsync().ContinueWith(async (message) => await message.Result.DeleteAsync());
        }
    }
}