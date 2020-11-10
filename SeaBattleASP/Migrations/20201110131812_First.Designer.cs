﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SeaBattleASP.Helpers;

namespace SeaBattleASP.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20201110131812_First")]
    partial class First
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SeaBattleASP.Models.Cell", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Color");

                    b.Property<int>("State");

                    b.Property<int>("X");

                    b.Property<int>("Y");

                    b.HasKey("Id");

                    b.ToTable("Cells");
                });

            modelBuilder.Entity("SeaBattleASP.Models.Deck", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsHead");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.ToTable("Decks");
                });

            modelBuilder.Entity("SeaBattleASP.Models.DeckCell", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CellId");

                    b.Property<int?>("DeckId");

                    b.Property<int?>("ShipId");

                    b.HasKey("Id");

                    b.HasIndex("CellId");

                    b.HasIndex("DeckId");

                    b.HasIndex("ShipId");

                    b.ToTable("DeckCells");
                });

            modelBuilder.Entity("SeaBattleASP.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsPl1Turn");

                    b.Property<int?>("Player1Id");

                    b.Property<int?>("Player2Id");

                    b.Property<int?>("PlayingFieldId");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.HasIndex("Player1Id");

                    b.HasIndex("Player2Id");

                    b.HasIndex("PlayingFieldId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("SeaBattleASP.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("SeaBattleASP.Models.PlayingField", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Heigth");

                    b.Property<int>("Width");

                    b.HasKey("Id");

                    b.ToTable("PlayingField");
                });

            modelBuilder.Entity("SeaBattleASP.Models.PlayingShip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ShipId");

                    b.Property<int>("ShipType");

                    b.HasKey("Id");

                    b.HasIndex("ShipId");

                    b.ToTable("PlayingShips");
                });

            modelBuilder.Entity("SeaBattleASP.Models.Ship", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("IsSelectedShip");

                    b.Property<int?>("PlayerId");

                    b.Property<int?>("PlayingFieldId");

                    b.Property<int>("Range");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("PlayingFieldId");

                    b.ToTable("Ship");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Ship");
                });

            modelBuilder.Entity("SeaBattleASP.Models.AuxiliaryShip", b =>
                {
                    b.HasBaseType("SeaBattleASP.Models.Ship");


                    b.ToTable("AuxiliaryShip");

                    b.HasDiscriminator().HasValue("AuxiliaryShip");
                });

            modelBuilder.Entity("SeaBattleASP.Models.MilitaryShip", b =>
                {
                    b.HasBaseType("SeaBattleASP.Models.Ship");


                    b.ToTable("MilitaryShip");

                    b.HasDiscriminator().HasValue("MilitaryShip");
                });

            modelBuilder.Entity("SeaBattleASP.Models.MixShip", b =>
                {
                    b.HasBaseType("SeaBattleASP.Models.Ship");


                    b.ToTable("MixShip");

                    b.HasDiscriminator().HasValue("MixShip");
                });

            modelBuilder.Entity("SeaBattleASP.Models.DeckCell", b =>
                {
                    b.HasOne("SeaBattleASP.Models.Cell", "Cell")
                        .WithMany()
                        .HasForeignKey("CellId");

                    b.HasOne("SeaBattleASP.Models.Deck", "Deck")
                        .WithMany()
                        .HasForeignKey("DeckId");

                    b.HasOne("SeaBattleASP.Models.Ship")
                        .WithMany("DeckCells")
                        .HasForeignKey("ShipId");
                });

            modelBuilder.Entity("SeaBattleASP.Models.Game", b =>
                {
                    b.HasOne("SeaBattleASP.Models.Player", "Player1")
                        .WithMany()
                        .HasForeignKey("Player1Id");

                    b.HasOne("SeaBattleASP.Models.Player", "Player2")
                        .WithMany()
                        .HasForeignKey("Player2Id");

                    b.HasOne("SeaBattleASP.Models.PlayingField", "PlayingField")
                        .WithMany()
                        .HasForeignKey("PlayingFieldId");
                });

            modelBuilder.Entity("SeaBattleASP.Models.PlayingShip", b =>
                {
                    b.HasOne("SeaBattleASP.Models.Ship", "Ship")
                        .WithMany()
                        .HasForeignKey("ShipId");
                });

            modelBuilder.Entity("SeaBattleASP.Models.Ship", b =>
                {
                    b.HasOne("SeaBattleASP.Models.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");

                    b.HasOne("SeaBattleASP.Models.PlayingField")
                        .WithMany("PlayingShips")
                        .HasForeignKey("PlayingFieldId");
                });
#pragma warning restore 612, 618
        }
    }
}