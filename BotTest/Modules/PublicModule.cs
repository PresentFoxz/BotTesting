using Discord;
using Discord.Commands;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TextCommandFramework.Services;

namespace TextCommandFramework.Modules;

// Modules must be public and inherit from an IModuleBase
public class PublicModule : ModuleBase<SocketCommandContext>
{
    private readonly BotContext _db;

    public PublicModule(BotContext db)
    {
        _db = db;
    }

    [Command("new")]
    public async Task NewAsync()
    {
        var user = await _db.Profile.FirstOrDefaultAsync(user => user.DiscordId == Context.User.Id);

        if (user != null)
        {
            await ReplyAsync("You already have a profile!");
            return;
        }
        else
        {
            var profile = new Profile
            {
                Name = Context.User.GlobalName,
                DiscordId = Context.User.Id,
                Money = 100,
                Level = 1,
                Experience = 0,
                InventorySpace = 10
            };

            _db.Profile.Add(profile);
            await _db.SaveChangesAsync();
            await ReplyAsync("Account created!");
        }
    }

    [Command("ping")]
    public Task PingAsync()
        => ReplyAsync("pong!");

    // Get info on a user, or the user who invoked the command if one is not specified
    [Command("userinfo")]
    public async Task UserInfoAsync(IUser user = null)
    {
        user ??= Context.User;

        await ReplyAsync(user.ToString());
    }
}
