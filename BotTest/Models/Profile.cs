using System;
using System.Collections.Generic;

namespace TextCommandFramework.Models;

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