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
                    Money = 10,
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
                            value = 1;
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
        var value1 = 0;
        var value2 = 0;
        var value3 = 0;
        var damage1 = 0;
        var damage2 = 0;
        var damage3 = 0;
        var itemId1 = 0;
        var itemId2 = 0;
        var itemId3 = 0;

        if (user != null && mess1 == "Sell")
        {
            user.Money += user.Value[user.ItemSelected];
            user.Inventory[user.ItemSelected] = 0;
            user.Damage[user.ItemSelected] = 0;
            user.Value[user.ItemSelected] = 0;
            await ReplyAsync($"You sold your weapon for {user.Value[user.ItemSelected]} gold!");
            return;
        }

        if (user != null && mess1 == "View")
        {
            List<string> nameList = new List<string> { "Nothing", "Sword", "Spear", "Axe", "GreatSword", "Rock", "Dagger" };
            string name = "";
            await ReplyAsync("Here is the current shop stock:");
            
            Random rnd1 = new Random();

            switch (rnd1.Next(0, 6))
            {
                case 1:
                    name = nameList[1];
                    damage1 = 5;
                    value1 = 10;
                    itemId1 = 1;
                    break;
                case 2:
                    name = nameList[2];
                    damage1 = 4;
                    value1 = 8;
                    itemId1 = 2;
                    break;
                case 3:
                    name = nameList[3];
                    damage1 = 5;
                    value1 = 12;
                    itemId1 = 3;
                    break;
                case 4:
                    name = nameList[4];
                    damage1 = 8;
                    value1 = 20;
                    itemId1 = 4;
                    break;
                case 5:
                    name = nameList[5];
                    damage1 = 2;
                    value1 = 1;
                    itemId1 = 5;
                    break;
                case 6:
                    name = nameList[6];
                    damage1 = 3;
                    value1 = 5;
                    itemId1 = 6;
                    break;
            }

            await ReplyAsync($"Item 1: {name} - {damage1} damage. Costs {value1} gold.");

            switch (rnd1.Next(0, 6))
            {
                case 1:
                    name = nameList[1];
                    damage2 = 5;
                    value2 = 10;
                    itemId2 = 1;
                    break;
                case 2:
                    name = nameList[2];
                    damage2 = 4;
                    value2 = 8;
                    itemId2 = 2;
                    break;
                case 3:
                    name = nameList[3];
                    damage2 = 5;
                    value2 = 12;
                    itemId2 = 3;
                    break;
                case 4:
                    name = nameList[4];
                    damage2 = 8;
                    value2 = 20;
                    itemId2 = 4;
                    break;
                case 5:
                    name = nameList[5];
                    damage2 = 2;
                    value2 = 1;
                    itemId2 = 5;
                    break;
                case 6:
                    name = nameList[6];
                    damage2 = 3;
                    value2 = 5;
                    itemId2 = 6;
                    break;
            }

            await ReplyAsync($"Item 2: {name} - {damage2} damage. Costs {value2} gold.");

            switch (rnd1.Next(0, 6))
            {
                case 1:
                    name = nameList[1];
                    damage3 = 5;
                    value3 = 10;
                    itemId3 = 1;
                    break;
                case 2:
                    name = nameList[2];
                    damage3 = 4;
                    value3 = 8;
                    itemId3 = 2;
                    break;
                case 3:
                    name = nameList[3];
                    damage3 = 5;
                    value3 = 12;
                    itemId3 = 3;
                    break;
                case 4:
                    name = nameList[4];
                    damage3 = 8;
                    value3 = 20;
                    itemId3 = 4;
                    break;
                case 5:
                    name = nameList[5];
                    damage3 = 2;
                    value3 = 1;
                    itemId3 = 5;
                    break;
                case 6:
                    name = nameList[6];
                    damage3 = 3;
                    value3 = 5;
                    itemId3 = 6;
                    break;
            }

            await ReplyAsync($"Item 3: {name} - {damage3} damage. Costs {value3} gold.");
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
                         "\r\n  ( Input a number from 1 - 10 )." +
                         "\r\n\r\n!Shop" +
                         "\r\n      Sell: Sells the weapon you are currently using." +
                         "\r\n      View: Shows the weapons you can buy. Shop stock is randomly generated." +
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