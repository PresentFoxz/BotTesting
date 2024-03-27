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

    [Command("Game")]
    public async Task GameAsync(string mess1, string mess2)
    {
        var user = await _db.Profile.FirstOrDefaultAsync(user => user.DiscordId == Context.User.Id);
        Profile profile = user;
        string name = "";
        int damage = 0;
        int value = 0;
        int detect = 0;

        Random rnd = new Random();
        List<string> nameList = new List<string> { "Nothing", "Sword", "Spear", "Axe", "GreatSword", "Rock", "Dagger" };

        if (mess1 == "Account")
        {
            if (user != null && mess2 == "New")
            {
                await ReplyAsync("You already have a profile!");
                return;
            }
            else if ( user == null && mess2 == "New")
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

            if (user != null && mess2 == "Delete")
            {
                _db.Profile.Remove(profile);
                await _db.SaveChangesAsync();
                await ReplyAsync("Account removed!");
                return;
            }

            if (user != null && mess2 == "ShowProfile")
            {
                await ReplyAsync($"This is you: {user.Name}, \nMoney: {user.Money} \nLevel: {user.Level} \nExperience: {user.Experience} \nSpace: {user.Inventory.Count - 1}");
                return;
            }

            if (user == null && mess2 != "New")
            {
                await ReplyAsync("Account not found!");
                return;
            }
        }

        if (mess1 == "Inventory")
        {
            if (mess2 == "CheckInv")
            {
                for (int i = 0; i < user.Inventory.Count - 1; i++)
                {
                    if (user.ItemSelected == i)
                    {
                        await ReplyAsync($"{i + 1}: {nameList[user.Inventory[i]]} ( Using )");
                    }
                    else
                    {
                        await ReplyAsync($"{i + 1}: {nameList[user.Inventory[i]]}");
                    }
                }
                return;
            }
        }

        if (mess1 == "SetItem")
        {
            if (mess2 == "Remove" && user.Inventory[user.Inventory.Count] > 0)
            {
                user.Inventory[user.Inventory.Count] = 0;
                user.Damage[user.Damage.Count] = 0;
                user.Value[user.Value.Count] = 0;
            }
        }

        if (mess1 == "Dungeon")
        {
            if (user != null && mess2 == "Crawl" && user.Fight < 0)
            {
                int Move = rnd.Next(1, 30);

                if (Move < 20)
                {
                    await ReplyAsync("You moved, but at what cost?");
                    return;
                }

                if (mess2 == "Crawl" && user.Fight >= 0)
                {
                    await ReplyAsync("You're in a Fight! --> !Game dungeon Fight");
                    return;
                }
                else if (Move > 20)
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
                    }

                    ;
                }

                await ReplyAsync($"You're in a Fight with: {user.CName}! --> !Game dungeon Fight");
                return;
            }

            if (user != null && mess2 == "Fight" && user.Fight >= 0)
            {
                int DMult = (user.Damage[user.ItemSelected] * user.Level * user.Value[user.ItemSelected]);
                user.CHP -= (DMult);

                await ReplyAsync($"You swung at your opponent and did {DMult} damage!");
                await ReplyAsync($"Your Hp: {user.Hp}");
                await ReplyAsync($"{user.CName}s Hp: {user.CHP}");

                if (user.CHP <= 0)
                {
                    await ReplyAsync($"You win! Here's the exp you've earned: {user.CExpGain}");
                    int random = rnd.Next(0, 6);

                    switch (random)
                    {
                        case 0:
                            name = nameList[0];
                            damage = 1;
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
                            name = nameList[5];
                            damage = 2;
                            value = 1;
                            break;
                        case 6:
                            name = nameList[6];
                            damage = 3;
                            value = 5;
                            break;
                    }

                    ;

                    for (int i = 0; i < user.Inventory.Count - 1; i++)
                    {
                        if (user.Inventory[i] == 0)
                        {
                            for (int l = 0; l < nameList.Count; l++)
                            {
                                if (nameList[l] == name)
                                {
                                    user.Inventory[i] = l;
                                }
                            }

                            user.Damage[i] = damage;
                            user.Value[i] = value;
                            detect = 1;
                            break;
                        }
                        else
                        {
                            detect = 0;
                        }
                    }

                    await ReplyAsync($"You found {name}! It does {damage} damage and is worth {value} gold!");

                    if (detect == 0)
                    {
                        await ReplyAsync(
                            $"Your inventory is full! Use ( !SetItem [ Space size ] ) to swap an item with what you just found!");
                        await ReplyAsync(
                            $"Definitely go check ( !Game Inventory CheckInv ) to see what you want to swap it with!");
                        await ReplyAsync(
                            $"If you don't see anything you wanna swap it with, type in ( !Game SetItem Remove )!");

                        for (int l = 0; l < nameList.Count; l++)
                        {
                            if (nameList[l] == name)
                            {
                                user.Inventory[user.Inventory.Count] = l;
                            }
                        }

                        user.Damage[user.Damage.Count] = damage;
                        user.Value[user.Value.Count] = value;
                    }

                    user.Fight = -1;
                }
                return;
            }
            else if (user != null && mess2 == "Fight" && user.Fight == -1)
            {
                await ReplyAsync("You just swung at mid air like a crazy man! Are you shadow boxing?");
            }
        }
    }

    [Command("Help")]
    public async Task HelpAsync()
    {
        await ReplyAsync("You will always put a space between your commands!" +
                         "\r\nTo use a command, try something like this! --> !Game account new" +
                         "\r\n\r\n!Game" +
                         "\r\n  Account:" +
                         "\r\n      New: Creates an account for said user." +
                         "\r\n      ShowProfile: Shows the details of your account." +
                         "\r\n      Delete: Deletes the profile you own." +
                         "\r\n  Dungeon:" +
                         "\r\n      Crawl: Moves you around in the dungeon." +
                         "\r\n      Fight: Fights the monster you're currently boxing." +
                         "\r\n  Inventory:" +
                         "\r\n      CheckInv: Checks the weapons you have in your Inventory." +
                         "\r\n      SwapHand: Swaps the weapon you are using for the one you want to swap with." +
                         "\r\n  SetItem: This is used for the item that you have collected recently." +
                         "\r\n      Remove: Removes the item you currently found after fighting ( use this if u don't want the item )." +
                         "\r\n\r\n!SetItem: Another cmd for swapping the items." +
                         "\r\n  ( Input a number from 1 - 10 ).");

    }

    [Command("SetItem")]
    public async Task ItemAsync(int num)
    {
        var user = await _db.Profile.FirstOrDefaultAsync(user => user.DiscordId == Context.User.Id);

        if (user != null && user.Inventory[user.Inventory.Count - 1] > 0)
        {
            if (user.Inventory.Count - 1 > num && num > 0)
            {
                user.Inventory[num] = user.Inventory[user.Inventory.Count];
                user.Damage[num] = user.Damage[user.Damage.Count];
                user.Value[num] = user.Value[user.Value.Count];

                user.Inventory[user.Inventory.Count] = 0;
                user.Damage[user.Damage.Count] = 0;
                user.Value[user.Value.Count] = 0;
            }
        }
    }
}
