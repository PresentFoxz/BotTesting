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
using System.Collections.Generic;
using System;
using TextCommandFramework.Models;
using static System.Net.Mime.MediaTypeNames;

namespace TextCommandFramework.Modules;

// Modules must be public and inherit from an IModuleBase
public class PublicModule : ModuleBase<SocketCommandContext>
{
    private readonly BotContext _db;

    public PublicModule(BotContext db)
    {
        _db = db;
    }

    [Command("test")]
    public async Task testAsync()
    {
        var user = await _db.Profile.FirstOrDefaultAsync(user => user.DiscordId == Context.User.Id);
        _db.Profile.Remove(user);
        await _db.SaveChangesAsync();
        return;
    }

    [Command("Game")]
    public async Task GameAsync(string mess1, string mess2)
    {
        var user = await _db.Profile.FirstOrDefaultAsync(user => user.DiscordId == Context.User.Id);
        Profile profile = user;
        string name = "";
        int damage = 0;
        int value = 0;

        Random rnd = new Random();
        List<string> nameList = new List<string> { "Nothing", "Sword", "Spear", "Axe", "GreatSword", "Rock", "Dagger"};

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
                    Level = 1
                };

                _db.Profile.Add(profile);
                await _db.SaveChangesAsync();
                await ReplyAsync("Account created!");
                return;
            }

            if (user != null && mess2 == "delete")
            {
                _db.Profile.Remove(profile);
                await _db.SaveChangesAsync();

                await ReplyAsync("Account reMoved!");
                return;
            }

            if (user != null && mess2 == "showProfile")
            {
                await ReplyAsync($"This is you: {user.Name}, \nMoney: {user.Money} \nLevel: {user.Level} \nExperience: {user.Experience} \nSpace: {user.Inventory.Count}");
                return;
            }

            if (user == null && mess2 == "delete" || mess2 == "showProfile")
            {
                await ReplyAsync("Account not found!");
                return;
            }
        }

        if (mess1 == "Inventory")
        {
            if (mess2 == "checkInv")
            {
                for (int i = 0; i < user.Inventory.Count; i++)
                {
                    await ReplyAsync($"{i + 1}: {nameList[i]}");
                    return;
                }
            }
        }

        if (mess1 == "Dungeon")
        {
            if (user != null && mess2 == "crawl" && user.Fight < 0)
            {
                int Move = rnd.Next(1, 30);

                if (Move > 20)
                {
                    user.Fight = rnd.Next(0, 3);

                    // 0: Chicken, 1: Bee, 2: Poisonous Spider, 3: Wolf
                    switch (user.Fight)
                    {
                        case 0:
                            user.CName = "Chicken";
                            user.CHP = 3;
                            user.CDamage = 1;
                            user.CExpGain = 5;
                            break;
                        case 1:
                            user.CName = "Bee";
                            user.CHP = 8;
                            user.CDamage = 2;
                            user.CExpGain = 7;
                            break;
                        case 2:
                            user.CName = "Poisonous Spider";
                            user.CHP = 12;
                            user.CDamage = 3;
                            user.CExpGain = 12;
                            break;
                        case 3:
                            user.CName = "Wolf";
                            user.CHP = 20;
                            user.CDamage = 10;
                            user.CExpGain = 25;
                            break;
                    };

                }
                return;
            }
            else if (user != null && mess2 == "crawl" && user.Fight >= 0)
            {
                await ReplyAsync("You're in a Fight! --> !Game dungeon Fight");
                return;
            }

            if (user != null && mess2 == "Fight" && user.Fight >= 0)
            {
                int DMult = (damage * user.Level * value);

                 user.CHP -= (DMult);

                if (user.CHP <= 0)
                {
                    await ReplyAsync($"You win! Here's the exp you've earned: {user.CExpGain}");

                    var item = new Items();
                    Items newItem = item;
                    int random = rnd.Next(0, 6);

                    switch (random)
                    {
                        case 0:
                            name = nameList[0];
                            damage = 0;
                            value = 0;
                            break;
                        case 1:
                            name = nameList[1];
                            damage = 5;
                            value = 10;
                            break;
                        case 2:
                            name = nameList[2];
                            damage = 4;
                            value = 8;
                            break;
                        case 3:
                            name = nameList[3];
                            damage = 5;
                            value = 12;
                            break;
                        case 4:
                            name = nameList[4];
                            damage = 8;
                            value = 20;
                            break;
                        case 5:
                            name = nameList[0];
                            damage = 2;
                            value = 1;
                            break;
                        case 6:
                            name = nameList[0];
                            damage = 3;
                            value = 5;
                            break;
                    };

                    item.ItemId = random;
                    item.Name = name;
                    item.Damage = damage;
                    item.Value = value;

                    await ReplyAsync($"You found {newItem.Name}! It does {newItem.Damage} damage and is worth {newItem.Value} gold!");

                    user.Fight = -1;
                }
                return;
            }
            else
            {
                await ReplyAsync("You just swung at mid air like a crazy man! Are you shadow boxing?");
                return;
            }
        }
    }

    [Command("Help")]
    public async Task HelpAsync()
    {
        await ReplyAsync("You will always put a space between your commands!" +
                         "\r\nTo use a command, try something like this! --> !Game account new" +
                         "\r\n\r\n!Game" +
                         "\r\n  account:" +
                         "\r\n      new: Creates an account for said user." +
                         "\r\n      showProfile: Shows the details of your account." +
                         "\r\n      delete: Deletes the profile you own." +
                         "\r\n  dungeon:" +
                         "\r\n      crawl: Moves you around in the dungeon." +
                         "\r\n      Fight: Fights the monster you're currently boxing." +
                         "\r\n  Inventory:" +
                         "\r\n      checkInv: Checks the weapons you have in your Inventory." +
                         "\r\n      swapHand: Swaps the weapon you are using for the one you want to swap with.");
    }
}