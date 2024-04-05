﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TextCommandFramework;

#nullable disable

namespace TextCommandFramework.Migrations
{
    [DbContext(typeof(BotContext))]
    [Migration("20240405141851_rah")]
    partial class rah
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("TextCommandFramework.Models.Profile", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("CDamage")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CExpGain")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CHP")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Damage")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("DiscordId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Experience")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Fight")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Hp")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Inventory")
                        .HasColumnType("TEXT");

                    b.Property<int>("ItemSelected")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Money")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfileId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserListId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserListId");

                    b.ToTable("Profile");
                });

            modelBuilder.Entity("TextCommandFramework.Models.UserList", b =>
                {
                    b.Property<string>("UserListId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserListId");

                    b.ToTable("List");
                });

            modelBuilder.Entity("TextCommandFramework.Models.Weapon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Damage")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Weapon");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Damage = 1,
                            Level = 1,
                            Name = "Fists",
                            Value = 0
                        },
                        new
                        {
                            Id = 2,
                            Damage = 5,
                            Level = 1,
                            Name = "Sword",
                            Value = 2
                        },
                        new
                        {
                            Id = 3,
                            Damage = 5,
                            Level = 1,
                            Name = "Spear",
                            Value = 3
                        },
                        new
                        {
                            Id = 4,
                            Damage = 7,
                            Level = 1,
                            Name = "Axe",
                            Value = 5
                        },
                        new
                        {
                            Id = 5,
                            Damage = 12,
                            Level = 1,
                            Name = "GreatSword",
                            Value = 8
                        },
                        new
                        {
                            Id = 6,
                            Damage = 200000,
                            Level = 1,
                            Name = "Rock",
                            Value = 10000
                        },
                        new
                        {
                            Id = 7,
                            Damage = 3,
                            Level = 1,
                            Name = "Dagger",
                            Value = 4
                        });
                });

            modelBuilder.Entity("TextCommandFramework.Models.Profile", b =>
                {
                    b.HasOne("TextCommandFramework.Models.UserList", null)
                        .WithMany("Profiles")
                        .HasForeignKey("UserListId");
                });

            modelBuilder.Entity("TextCommandFramework.Models.UserList", b =>
                {
                    b.Navigation("Profiles");
                });
#pragma warning restore 612, 618
        }
    }
}
