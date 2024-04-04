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
    public async Task GameAsync(string subCommand, string mess2, string nameLookup)
    {
        var profile = await _db.Profile.FirstOrDefaultAsync(usr => usr.DiscordId == Context.User.Id);
        var weapons = await _db.Weapon.OrderBy(w => w.Id).Select(w => w.Name).ToListAsync();
        var id = await _db.Weapon.OrderBy(w => w.Id).Select(w => w.Id).ToListAsync();
        var value = await _db.Weapon.OrderBy(w => w.Id).Select(w => w.Value).ToListAsync();
        var damage = await _db.Weapon.OrderBy(w => w.Id).Select(w => w.Damage).ToListAsync();

        if (subCommand == "none")
        {
            await ReplyAsync("You moved, but at what cost?");
            return;
        }
        switch (subCommand)
        {
            case "Account":
                await HandleGameAccountAsync(profile, mess2, nameLookup, weapons, id, damage, value);
                break;

            case "Inventory":
                await HandleInventoryAsync(profile, mess2, nameLookup, weapons, id, damage, value);
                break;

            case "Dungeon":
                await HandleDungeonAsync(profile, mess2, nameLookup, weapons, id, damage, value);
                break;

            case "SetItem":
                await HandleSetItemAsync(profile, mess2, nameLookup, weapons, id, damage, value);
                break;

            case "AllItems":
                await HandleAllItemsAsync(profile, mess2, nameLookup, weapons, id, damage, value);
                break;

            default:
                await ReplyAsync("Unknown command entered " + subCommand);
                break;
        }
    }

    public async Task HandleAllItemsAsync(Profile profile, string mess2, string nameLookup, List<string> weapons, List<int> id, List<int> damage,
        List<int> value)
    {
        if (mess2 == "All")
        {
            await ReplyAsync($"Here are the items in the shop!");
            for (int i = 0; i < weapons.Count; i++)
            {
                await ReplyAsync($"{i}: ?");
            }

            return;
        }
    }

    public async Task HandleSetItemAsync(Profile profile, string mess2, string nameLookup, List<string> weapons, List<int> id, List<int> damage, List<int> value)
    {
        if (mess2 == "Remove" && profile.Inventory[profile.Inventory.Count] > 0)
        {
            profile.Inventory[profile.Inventory.Count] = 0;
            profile.Damage[profile.Damage.Count] = 0;
            profile.Value[profile.Value.Count] = 0;
        }
    }

    public async Task HandleDungeonAsync(Profile profile, string mess2, string nameLookup, List<string> weapons, List<int> id, List<int> damage, List<int> value)
    {
        Random rnd = new Random();

        int detect = 0;

        if (profile != null && mess2 == "Crawl" && profile.Fight < 0)
        {
            int Move = rnd.Next(1, 30);

            if (Move < 20)
            {
                await ReplyAsync("You moved, but at what cost?");
                return;
            }

            if (mess2 == "Crawl" && profile.Fight >= 0)
            {
                await ReplyAsync("You're in a Fight! --> !Game dungeon Fight");
                return;
            }
            else if (Move > 20)
            {
                profile.Fight = rnd.Next(0, 3);

                // 0: Chicken, 1: Bee, 2: Poisonous Spider, 3: Wolf
                switch (profile.Fight)
                {
                    case 0:
                        profile.CName = "Chicken";
                        profile.CHP = 3;
                        profile.CDamage = 1;
                        profile.CExpGain = 5;
                        break;
                    case 1:
                        profile.CName = "Bee";
                        profile.CHP = 8;
                        profile.CDamage = 2;
                        profile.CExpGain = 7;
                        break;
                    case 2:
                        profile.CName = "Poisonous Spider";
                        profile.CHP = 12;
                        profile.CDamage = 3;
                        profile.CExpGain = 12;
                        break;
                    case 3:
                        profile.CName = "Wolf";
                        profile.CHP = 20;
                        profile.CDamage = 10;
                        profile.CExpGain = 25;
                        break;
                }

                ;
            }

            await ReplyAsync($"You're in a Fight with: {profile.CName}! --> !Game dungeon Fight");
            return;
        }

        if (profile != null && mess2 == "Fight" && profile.Fight >= 0)
        {
            int DMult = (profile.Damage[profile.ItemSelected] * profile.Level * profile.Value[profile.ItemSelected]);
            profile.CHP -= (DMult);

            await ReplyAsync($"You swung at your opponent and did {DMult} damage!");
            await ReplyAsync($"Your Hp: {profile.Hp}");
            await ReplyAsync($"{profile.CName}s Hp: {profile.CHP}");

            if (profile.CHP <= 0)
            {
                await ReplyAsync($"You win! Here's the exp you've earned: {profile.CExpGain}");
                profile.Experience += profile.CExpGain;
                profile.CExpGain = 0;
                int random = rnd.Next(0, 6);

                profile.Inventory[10] = id[random];
                profile.Damage[10] = damage[random];
                profile.Value[10] = value[random];

                await ReplyAsync($"You found {id[random]}!" +
                                 $"\rIt does {damage[random]} damage!" +
                                 $"\rIt has a value of {value[random]}");

                if (detect == 0)
                {
                    await ReplyAsync(
                        "Your inventory is full! Use ( !SetItem [ Space size ] ) to swap an item with what you just found!" +
                        "\rDefinitely go check ( !Game Inventory CheckInv ) to see what you want to swap it with!" +
                        "\rIf you don't see anything you wanna swap it with, type in ( !Game SetItem Remove )!");
                }

                profile.Fight = -1;
            }
            return;
        }
        else if (profile != null && mess2 == "Fight" && profile.Fight == -1)
        {
            await ReplyAsync("You just swung at mid air like a crazy man! Are you shadow boxing?");
        }
    }
    public async Task HandleInventoryAsync(Profile profile, string mess2, string nameLookup, List<string> weapons, List<int> id, List<int> damage, List<int> value)
    {
        if (mess2 == "CheckInv")
        {
            for (int i = 0; i < profile.Inventory.Count - 1; i++)
            {
                if (profile.ItemSelected == i)
                {
                    await ReplyAsync($"{i + 1}: {i} ( Using )");
                }
                else
                {
                    await ReplyAsync($"{i + 1}: {i}");
                }
            }

            return;
        }
    }

    public async Task HandleGameAccountAsync(Profile profile, string mess2, string nameLookup, List<string> weapons, List<int> id, List<int> damage, List<int> value)
    {
        if (profile != null && mess2 == "New")
        {
            await ReplyAsync("You already have a profile!");
            return;
        }
        else if (profile == null && mess2 == "New")
        {
            var newProfile = new Profile
            {
                Name = Context.User.GlobalName,
                DiscordId = Context.User.Id,
                Money = 100,
                Level = 1
            };

            _db.Profile.Add(newProfile);
            await _db.SaveChangesAsync();
            await ReplyAsync("Account created!");
            return;
        }

        if (profile != null && mess2 == "Delete")
        {
            _db.Profile.Remove(profile);
            await _db.SaveChangesAsync();
            await ReplyAsync("Account removed!");
            return;
        }

        if (profile != null && mess2 == "ShowProfile")
        {
            await ReplyAsync($"This is you: {profile.Name}, \nMoney: {profile.Money} \nLevel: {profile.Level} \nExperience: {profile.Experience} \nSpace: {profile.Inventory.Count - 1}");
            return;
        }

        if (profile != null && mess2 == "ProfileLookup")
        {
            var other = await _db.Profile.FirstOrDefaultAsync(usr => usr.Name == nameLookup);

            if (other != null)
                await ReplyAsync($"This is you: {other.Name}, \nMoney: {other.Money} \nLevel: {other.Level} \nExperience: {other.Experience} \nSpace: {other.Inventory.Count - 1}");
            else
            {
                await ReplyAsync($"Sorry but I wasn't able to find {nameLookup}");
            }
            return;
        }

        if (profile == null && mess2 != "New")
        {
            await ReplyAsync("Account not found!");
            return;
        }
    }


    [Command("Help")]
    public async Task HelpAsync()
    {
        await ReplyAsync("You will always put a space between your commands!" +
                         "\r\nTo use a command, try something like this! --> !Game account new" +
                         "\r\n\r\n!Game" +
                         "\r\n  Account:" +
                         "\r\n      New: Creates an account for said profile." +
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

        if (user != null && (user.Inventory[user.Inventory.Count - 1] >= 1 || user.Inventory[user.Inventory.Count - 1] <= 10))
        {
            if (user.Inventory.Count - 1 > num && (num >= 1 || num <= 10)) 
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
