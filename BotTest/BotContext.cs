using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace TextCommandFramework;

public class BotContext : DbContext
{
    public DbSet<Profile> Profile { get; set; }
    public DbSet<UserList> List { get; set; }

    public BotContext(DbContextOptions<BotContext> options) : base(options)
    {
    }
}

public class UserList
{
    public string UserListId { get; set; }
    public List<Profile> Profiles { get; set; }
}

public class Profile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProfileId { get; set; }
    public string Name { get; set; }
    public ulong DiscordId { get; set; }
    public int Money { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public List<int> Inventory { get; set; }
    public int Fight { get; set; }
    public string CName { get; set; }
    public int CExpGain { get; set; }
    public int CHP { get; set; }
    public int CDamage { get; set; }
}

public class Items
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int ItemId { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }
    public int Value { get; set; }
}