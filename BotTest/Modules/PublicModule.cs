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
using TextCommandFramework.Models;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace TextCommandFramework.Modules;

// Modules must be public and inherit from an IModuleBase
public class PublicModule : ModuleBase<SocketCommandContext>
{
    private readonly BotContext _db;
    public PublicModule(BotContext db)
    {
        _db = db;
    }

    /*
    [Command("HateMail")]
    public async Task HateMailAsync()
    {
        await ReplyAsync("kys");
    }
    */
    [Command("Game")]
    public async Task GameAsync(string subCommand = "", string mess2 = "", string nameLookup = "")
    {
        var profile = await _db.Profile.FirstOrDefaultAsync(usr => usr.DiscordId == Context.User.Id);
        var weapons = await _db.Weapon.OrderBy(w => w.Id).ToListAsync();
        
        switch (subCommand)
        {
            case "Account":
                await HandleGameAccountAsync(profile, mess2, nameLookup, weapons);
                break;

            case "Inventory":
                await HandleInventoryAsync(profile, mess2, nameLookup, weapons);
                break;

            case "Dungeon":
                await HandleDungeonAsync(profile, mess2, nameLookup, weapons);
                break;

            case "SetItem":
                await HandleSetItemAsync(profile, mess2, nameLookup, weapons);
                break;

            case "AllItems":
                await HandleAllItemsAsync(profile, mess2, nameLookup, weapons);
                break;

            default:
                await ReplyAsync("Unknown command entered " + subCommand);
                break;
        }
    }

    public async Task HandleAllItemsAsync(Profile profile, string mess2, string nameLookup, List<Weapon> weapons)
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

    public async Task HandleSetItemAsync(Profile profile, string mess2, string nameLookup, List<Weapon> weapons)
    {
        if (mess2 == "Remove" && profile.Inventory[profile.Inventory.Count] > 0)
        {
            profile.Inventory[profile.Inventory.Count] = 0;
            profile.Damage[profile.Damage.Count] = 0;
            profile.Value[profile.Value.Count] = 0;
        }
    }

    public async Task HandleDungeonAsync(Profile profile, string mess2, string nameLookup, List<Weapon> weapons)
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

                profile.Inventory[10] = weapons[random].Id;
                profile.Damage[10] = weapons[random].Damage;
                profile.Value[10] = weapons[random].Value;

                await ReplyAsync($"You found {weapons[random].Id}!" +
                                 $"\rIt does {weapons[random].Damage} damage!" +
                                 $"\rIt has a value of {weapons[random].Value}");

                for (int i = 0; i < 9; i++)
                {
                    if (profile.Inventory[i] > 0)
                    {
                        detect = 1;
                    }
                }

                if (detect == 0)
                {
                    await ReplyAsync(
                        "Your inventory is full! Use ( !SetItem [ Space size ] ) to swap an item with what you just found!" +
                        "\rDefinitely go check ( !Game Inventory CheckInv ) to see what you want to swap it with!" +
                        "\rIf you don't see anything you wanna swap it with, type in ( !Game SetItem Remove )!");
                }

                detect = 0;
                profile.Fight = -1;
            }
            return;
        }
        else if (profile != null && mess2 == "Fight" && profile.Fight == -1)
        {
            await ReplyAsync("You just swung at mid air like a crazy man! Are you shadow boxing?");
        }
    }
    public async Task HandleInventoryAsync(Profile profile, string mess2, string nameLookup, List<Weapon> weapons)
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

    public async Task HandleGameAccountAsync(Profile profile, string mess2, string nameLookup, List<Weapon> weapons)
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

    // Adding/removing money, levels, etc. for testing purposes
    [Command("Test")]
    public async Task TestAsync(string mess1, string mess2, int amount)
    {
        var user = await _db.Profile.FirstOrDefaultAsync(user => user.DiscordId == Context.User.Id);

        if (user == null)
        {
            await ReplyAsync("You don't have an account! Create one with !Game account new");
        }

        if (mess1 == "Hp")
        {

            if (user != null && mess2 == "Add")
            {
                user.Hp += amount;
                await ReplyAsync($"Added {amount} HP");
            }
            else if (user != null && mess2 == "Remove")
            {
                user.Hp -= amount;
                await ReplyAsync($"Removed {amount} HP");
            }
        }

        if (mess1 == "Money")
        {

            if (user != null && mess2 == "Add")
            {
                user.Money += amount;
                await ReplyAsync($"Added {amount} gold");
            }
            else if (user != null && mess2 == "Remove")
            {
                user.Money -= amount;
                await ReplyAsync($"Removed {amount} gold");
            }
        }

        if (mess1 == "Level")
        {

            if (user != null && mess2 == "Add")
            {
                user.Level += amount;
                await ReplyAsync($"Added {amount} level(s)");
            }
            else if (user != null && mess2 == "Remove")
            {
                user.Level -= amount;
                await ReplyAsync($"Removed {amount} level(s)");
            }
        }

        if (mess1 == "Experience")
        {

            if (user != null && mess2 == "Add")
            {
                user.Experience += amount;
                await ReplyAsync($"Added {amount} experience");
            }
            else if (user != null && mess2 == "Remove")
            {
                user.Experience -= amount;
                await ReplyAsync($"Removed {amount} experience");
            }
        }
    }

    [Command("Shop")]
    public async Task ShopAsync(string mess1, int item)
    {
        var user = await _db.Profile.FirstOrDefaultAsync(user => user.DiscordId == Context.User.Id);
        var value1 = 0; var value2 = 0; var value3 = 0;
        var damage1 = 0; var damage2 = 0; var damage3 = 0;
        var itemId1 = 0; var itemId2 = 0; var itemId3 = 0;
        List<string> nameList = new List<string> { "Nothing", "Sword", "Spear", "Axe", "GreatSword", "Rock", "Dagger" };
        var name1 = ""; var name2 = ""; var name3 = "";

        Random rnd1 = new Random();
        switch (rnd1.Next(1, 6))
        {
            case 1:
                name1 = nameList[1];
                damage1 = 5;
                value1 = 10;
                itemId1 = 1;
                break;
            case 2:
                name1 = nameList[2];
                damage1 = 4;
                value1 = 8;
                itemId1 = 2;
                break;
            case 3:
                name1 = nameList[3];
                damage1 = 5;
                value1 = 12;
                itemId1 = 3;
                break;
            case 4:
                name1 = nameList[4];
                damage1 = 8;
                value1 = 20;
                itemId1 = 4;
                break;
            case 5:
                name1 = nameList[5];
                damage1 = 2;
                value1 = 1;
                itemId1 = 5;
                break;
            case 6:
                name1 = nameList[6];
                damage1 = 3;
                value1 = 5;
                itemId1 = 6;
                break;
        } // Item 1
        switch (rnd1.Next(1, 6))
        {
            case 1:
                name2 = nameList[1];
                damage2 = 5;
                value2 = 10;
                itemId2 = 1;
                break;
            case 2:
                name2 = nameList[2];
                damage2 = 4;
                value2 = 8;
                itemId2 = 2;
                break;
            case 3:
                name2 = nameList[3];
                damage2 = 5;
                value2 = 12;
                itemId2 = 3;
                break;
            case 4:
                name2 = nameList[4];
                damage2 = 8;
                value2 = 20;
                itemId2 = 4;
                break;
            case 5:
                name2 = nameList[5];
                damage2 = 2;
                value2 = 1;
                itemId2 = 5;
                break;
            case 6:
                name2 = nameList[6];
                damage2 = 3;
                value2 = 5;
                itemId2 = 6;
                break;
        } // Item 2
        switch (rnd1.Next(1, 6))
        {
            case 1:
                name3 = nameList[1];
                damage3 = 5;
                value3 = 10;
                itemId3 = 1;
                break;
            case 2:
                name3 = nameList[2];
                damage3 = 4;
                value3 = 8;
                itemId3 = 2;
                break;
            case 3:
                name3 = nameList[3];
                damage3 = 5;
                value3 = 12;
                itemId3 = 3;
                break;
            case 4:
                name3 = nameList[4];
                damage3 = 8;
                value3 = 20;
                itemId3 = 4;
                break;
            case 5:
                name3 = nameList[5];
                damage3 = 2;
                value3 = 1;
                itemId3 = 5;
                break;
            case 6:
                name3 = nameList[6];
                damage3 = 3;
                value3 = 5;
                itemId3 = 6;
                break;
        } // Item 3

        if (user != null && mess1 == "Sell")
        {
            user.Money += user.Value[user.ItemSelected];
            user.Inventory[user.ItemSelected] = 0;
            user.Damage[user.ItemSelected] = 0;
            user.Value[user.ItemSelected] = 0;
            await ReplyAsync($"You sold your weapon for {user.Value[user.ItemSelected]} gold!");
            return;
        }

        if (user != null && mess1 == "View" && item != null) // Supposed to always be true so whatever number the user enters won't matter
        {
            await ReplyAsync("Here is the current shop stock:");
            await ReplyAsync($"Item 1: {name1} - {damage1} damage. Costs {value1} gold.");
            await ReplyAsync($"Item 2: {name2} - {damage2} damage. Costs {value2} gold.");
            await ReplyAsync($"Item 3: {name3} - {damage3} damage. Costs {value3} gold.");
        }
        
        if (user != null && mess1 == "Buy" && item == 1)
        {
            if (user.Money >= user.Value[value1])
            {
                user.Money -= user.Value[value1];
                user.Inventory[user.ItemSelected] = itemId1;
                user.Damage[user.ItemSelected] = damage1;
                user.Value[user.ItemSelected] = value1;
                await ReplyAsync($"You bought the weapon for {value1} gold!");
            }
            else
            {
                await ReplyAsync("You don't have enough money!");
            }
        }
        
        if (user != null && mess1 == "Buy" && item == 2)
        {
            if (user.Money >= user.Value[value2])
            {
                user.Money -= user.Value[user.ItemSelected];
                user.Inventory[user.ItemSelected] = itemId2;
                user.Damage[user.ItemSelected] = damage2;
                user.Value[user.ItemSelected] = value2;
                await ReplyAsync($"You bought the weapon for {value2} gold!");
            }
            else
            {
                await ReplyAsync("You don't have enough money!");
            }
        }
        
        if (user != null && mess1 == "Buy" && item == 3)
        {
            if (user.Money >= user.Value[value3])
            {
                user.Money -= user.Value[user.ItemSelected];
                user.Inventory[user.ItemSelected] = itemId3;
                user.Damage[user.ItemSelected] = damage3;
                user.Value[user.ItemSelected] = value3;
                await ReplyAsync($"You bought the weapon for {value3}!");
            }
            else
            {
                await ReplyAsync("You don't have enough money!");
            }
        }

        if (user == null)
        {
            await ReplyAsync("You don't have an account! Create one with !Game account new");
        }
    }

    [Command("Help")]
    public async Task HelpAsync()
    {
        await ReplyAsync("You will always put a space between your commands!" +
                         "\r\nTo use a command, try something like this! --> !Game account new" +
                         "\r\n\r\n!Test" +
                         "\r\n  Hp:" +
                         "\r\n      Add: Adds HP to your account." +
                         "\r\n      Remove: Removes HP from your account." +
                         "\r\n  Money:" +
                         "\r\n      Add: Adds money to your account." +
                         "\r\n      Remove: Removes money from your account." +
                         "\r\n  Level:" +
                         "\r\n      Add: Adds levels to your account." +
                         "\r\n      Remove: Removes levels from your account." +
                         "\r\n  Experience:" +
                         "\r\n      Add: Adds experience to your account." +
                         "\r\n      Remove: Removes experience from your account." +
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
                         "\r\n  ( Input a number from 1 - 10 )." +
                         "\r\n\r\n!Shop" +
                         "\r\n      Sell: Sells the weapon you are currently using." +
                         "\r\n      View [Number]: Shows the weapons you can buy. Shop stock is randomly generated. Number can be anything" +
                         "\r\n      Buy [Number (1-3)]: Purchases a weapon. Be sure to swap to an empty spot in your inventory first.");

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