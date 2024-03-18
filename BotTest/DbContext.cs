using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class BotContext : DbContext
{
    public DbSet<Profile> Profile { get; set; }
    public DbSet<UserList> List { get; set; }

    public string DbPath { get; }
    public BotContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "bot.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class UserList
{
    public string UserListId { get; set; }
    public List<Profile> Profiles { get; set; }
}

public class Profile
{
    public string ProfileId { get; set; }
    public string Name { get; set; }
    public ulong DiscordId { get; set; }
    public int Money { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int InventorySpace { get; set; }
}