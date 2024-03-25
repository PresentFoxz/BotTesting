using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TextCommandFramework.Models;

namespace TextCommandFramework;

public class BotContext : DbContext
{
    public DbSet<Profile> Profile { get; set; }
    public DbSet<UserList> List { get; set; }

    public BotContext(DbContextOptions<BotContext> options) : base(options)
    {
    }
}

public class Items
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int ItemId { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }
    public int Value { get; set; }
}