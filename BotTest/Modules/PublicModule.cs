using Discord;
using Discord.Commands;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TextCommandFramework.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TextCommandFramework.Modules;

// Modules must be public and inherit from an IModuleBase
public class PublicModule : ModuleBase<SocketCommandContext>
{
    private readonly BotContext _db;

    public PublicModule(BotContext db)
    {
        _db = db;
    }

    [Command("Game")]
    public async Task GameAsync(string mess1, string mess2)
    {
        var user = await _db.Profile.FirstOrDefaultAsync(user => user.DiscordId == Context.User.Id);
        Profile profile = user;

        if (mess1 == "account")
        {
            if (user != null && mess2 == "new")
            {
                await ReplyAsync("You already have a profile!");
                return;
            }
            else if (user == null && mess2 == "new")
            {
                profile = new Profile
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

            if (user != null && mess2 == "delete")
            {
                _db.Profile.Remove(profile);
                _db.SaveChangesAsync();

                await ReplyAsync("Account removed!");
                return;
            }

            if (user != null && mess2 == "showProfile")
            {
                await ReplyAsync($"This is you: {user.Name}, \nMoney: {user.Money} \nLevel: {user.Level} \nExperience: {user.Experience} \nSpace: {user.InventorySpace}");
                return;
            }

            if (user == null && mess2 == "delete" || mess2 == "showProfile")
            {
                await ReplyAsync("Account not found!");
                return;
            }
        }

        if (mess1 == "Dungeon")
        {
            if (user != null && mess2 == "Crawl")
            {
                return;
            }
        }
    }

    [Command("Help")]
    public async Task HelpAsync()
    {
        await ReplyAsync("You will always put a space between your commands!" +
                         "\r\nTo use a command, try something like this! --> !Game account new" +
                         "\r\n\r\naccount:" +
                         "\r\n    new: Creates an account for said user." +
                         "\r\n    showProfile: Shows the details of your account." +
                         "\r\n    delete: Deletes the profile you own." +
                         "\r\ndungeon:" +
                         "\r\n    crawl: Moves you around in the dungeon.");
    }
}
