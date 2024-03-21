using Discord;
using Discord.Commands;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TextCommandFramework.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System;
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

    [Command("Game")]
    public async Task GameAsync(string mess1, string mess2)
    {
        var user = await _db.Profile.FirstOrDefaultAsync(user => user.DiscordId == Context.User.Id);
        Profile profile = user;
        
        string cName = "";
        int cExpGain, cHP, cDamage = 0;
        int fight = -1;
        Random rnd = new Random();

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

        if (mess1 == "inventory")
        {
            if (mess2 == "checkInv")
            {
                return;
            }
        }

        if (mess1 == "Dungeon")
        {
            if (user != null && mess2 == "crawl" && fight < 0)
            {
                int move = rnd.Next(1, 30);

                if (move > 20)
                {
                    fight = rnd.Next(0, 3);

                    // 0: Chicken, 1: Bee, 2: Poisonous Spider, 3: Wolf
                    switch (fight)
                    {
                        case 0:
                            cName = "Chicken";
                            cHP = 3;
                            cDamage = 1;
                            cExpGain = 5;
                            break;
                        case 1:
                            cName = "Bee";
                            cHP = 8;
                            cDamage = 2;
                            cExpGain = 7;
                            break;
                        case 2:
                            cName = "Poisonous Spider";
                            cHP = 12;
                            cDamage = 3;
                            cExpGain = 12;
                            break;
                        case 3:
                            cName = "Wolf";
                            cHP = 20;
                            cDamage = 10;
                            cExpGain = 25;
                            break;
                    };

                }
                return;
            }
            else if (user != null && mess2 == "crawl" && fight >= 0)
            {
                await ReplyAsync("You're in a fight! --> !Game dungeon fight");
                return;
            }

            if (user != null && mess2 == "fight" && fight >= 0)
            {
                int dMult = (damage * user.Level * value);

                 cHP -= (dMult);

                if (cHP <= 0)
                {
                    await ReplyAsync($"You win! Here's the exp you've earned: {cExpGain}");
                    fight = -1;
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
                         "\r\n      fight: Fights the monster you're currently boxing." +
                         "\r\n  inventory:" +
                         "\r\n      checkInv: Checks the weapons you have in your inventory." +
                         "\r\n      swapHand: Swaps the weapon you are using for the one you want to swap with.");
    }
}
