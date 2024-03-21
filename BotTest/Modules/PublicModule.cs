using System;
using Discord;
using Discord.Commands;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
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

    [Command("Test")]
    public async Task TestAsync()
    {
        var item = new Items();
        Items newItem = item;
        Random rnd = new Random();
        int random = rnd.Next(0, 6);
        string name = "";
        int damage = 0;
        int value = 0;

        switch (random)
        {
            case 0:
                name = "Nothing";
                damage = 1;
                value = 0;
                break;
            case 1:
                name = "a Sword";
                damage = 5;
                value = 10;
                break;
            case 2:
                name = "a Spear";
                damage = 4;
                value = 8;
                break;
            case 3:
                name = "a Axe";
                damage = 5;
                value = 12;
                break;
            case 4:
                name = "a Greatsword";
                damage = 8;
                value = 20;
                break;
            case 5:
                name = "a Rock";
                damage = 2;
                value = 1;
                break;
            case 6:
                name = "a Dagger";
                damage = 3;
                value = 5;
                break;
        };

        item.ItemId = random;
        item.Name = name;
        item.Damage = damage;
        item.Value = value;

        await ReplyAsync($"You found {newItem.Name}! It does {newItem.Damage} damage and is worth {newItem.Value} gold!");
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
                    InventorySpace = 10,
                    cName = "",
                    cExpGain = 0,
                    cHP = 0,
                    cDamage = 0,
                    fight = -1
                };

                _db.Profile.Add(profile);
                await _db.SaveChangesAsync();
                await ReplyAsync("Account created!");
            }
            if (user != null && mess2 == "delete")
            {
                _db.Profile.Remove(profile);
                await _db.SaveChangesAsync();
                await ReplyAsync("Account removed!");
                return;
            }
            else if (user == null && mess2 == "delete")
            {
                await ReplyAsync("Account not found!");
                return;
            }

            if (user != null && mess2 == "showProfile")
            {
                await ReplyAsync($"This is you: {user.Name}");
                return;
            }
        }

        if (mess1 == "dungeon")
        {
            if (user != null && mess2 == "crawl")
            {
                var item = new Items();
                Items newItem = item;
                Random rnd = new Random();
                int random = rnd.Next(0, 6);
                string name = "";
                int damage = 0;
                int value = 0;

                switch (random)
                {
                    case 0:
                        name = "Nothing";
                        damage = 0;
                        value = 0;
                        break;
                    case 1:
                        name = "a Sword";
                        damage = 5;
                        value = 10;
                        break;
                    case 2:
                        name = "a Spear";
                        damage = 4;
                        value = 8;
                        break;
                    case 3:
                        name = "an Axe";
                        damage = 5;
                        value = 12;
                        break;
                    case 4:
                        name = "a Greatsword";
                        damage = 8;
                        value = 20;
                        break;
                    case 5:
                        name = "a Rock";
                        damage = 2;
                        value = 1;
                        break;
                    case 6:
                        name = "a Dagger";
                        damage = 3;
                        value = 5;
                        break;
                };

                item.ItemId = random;
                item.Name = name;
                item.Damage = damage;
                item.Value = value;

                await ReplyAsync($"You found {newItem.Name}! It does {newItem.Damage} damage and is worth {newItem.Value} gold!");
            }


        }
    }
}