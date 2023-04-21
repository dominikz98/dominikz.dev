﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dominikz.Infrastructure.Provider;
using dominikz.Infrastructure.Provider.Database;

#nullable disable

namespace dominikz.Migrations.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.3");

            modelBuilder.Entity("dominikz.Domain.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Permissions")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RefreshExpiration")
                        .HasColumnType("TEXT");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("accounts", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.Article", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<string>("HtmlText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("articles", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.Food", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("CaloriesInKcal")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("CarbohydratesInG")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DietaryFiberInG")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("FatInG")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ProteinInG")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SaltInG")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SugarInG")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SupermarktCheckId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Unit")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("foods", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.FoodSnapshot", b =>
                {
                    b.Property<Guid>("FoodId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Release")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<string>("Store")
                        .HasColumnType("TEXT");

                    b.HasKey("FoodId", "Release");

                    b.ToTable("food_snapshots", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.Ingredient", b =>
                {
                    b.Property<Guid>("RecipeId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("FoodId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Factor")
                        .HasColumnType("TEXT");

                    b.HasKey("RecipeId", "FoodId");

                    b.HasIndex("FoodId");

                    b.ToTable("ingredients", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.Movies", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("medias", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("dominikz.Domain.Models.Recipe", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Portions")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("recipes", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.RecipeStep", b =>
                {
                    b.Property<Guid>("RecipeId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RecipeId", "Order");

                    b.ToTable("recipe_steps", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.Song", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("Bpm")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("songs", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.SongSegment", b =>
                {
                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("SongId")
                        .HasColumnType("TEXT");

                    b.Property<int>("BottomClef")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BottomNotes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("BottomTact")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TopClef")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TopNotes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TopTact")
                        .HasColumnType("INTEGER");

                    b.HasKey("Index", "SongId");

                    b.HasIndex("SongId");

                    b.ToTable("songs_segments", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.StorageFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Extension")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("files", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.Book", b =>
                {
                    b.HasBaseType("dominikz.Domain.Models.Movies");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Genres")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Language")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.ToTable("books", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.Game", b =>
                {
                    b.HasBaseType("dominikz.Domain.Models.Movies");

                    b.Property<int>("Genres")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Platform")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.ToTable("games", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.Movie", b =>
                {
                    b.HasBaseType("dominikz.Domain.Models.Movies");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .HasColumnType("TEXT");

                    b.Property<int>("Genres")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImdbId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("JustWatchId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Plot")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("Runtime")
                        .HasColumnType("TEXT");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.Property<string>("YoutubeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.ToTable("movies", (string)null);
                });

            modelBuilder.Entity("dominikz.Domain.Models.Article", b =>
                {
                    b.HasOne("dominikz.Domain.Models.StorageFile", "File")
                        .WithOne("Article")
                        .HasForeignKey("dominikz.Domain.Models.Article", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("dominikz.Domain.Models.FoodSnapshot", b =>
                {
                    b.HasOne("dominikz.Domain.Models.Food", null)
                        .WithMany("Snapshots")
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.Domain.Models.Ingredient", b =>
                {
                    b.HasOne("dominikz.Domain.Models.Food", "Food")
                        .WithMany()
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dominikz.Domain.Models.Recipe", "Recipe")
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Food");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("dominikz.Domain.Models.Movies", b =>
                {
                    b.HasOne("dominikz.Domain.Models.StorageFile", "File")
                        .WithOne("Movies")
                        .HasForeignKey("dominikz.Domain.Models.Movies", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("dominikz.Domain.Models.Recipe", b =>
                {
                    b.HasOne("dominikz.Domain.Models.StorageFile", "Image")
                        .WithOne("Recipe")
                        .HasForeignKey("dominikz.Domain.Models.Recipe", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");
                });

            modelBuilder.Entity("dominikz.Domain.Models.RecipeStep", b =>
                {
                    b.HasOne("dominikz.Domain.Models.Recipe", null)
                        .WithMany("Steps")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.Domain.Models.SongSegment", b =>
                {
                    b.HasOne("dominikz.Domain.Models.Song", "Song")
                        .WithMany("Segments")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Song");
                });

            modelBuilder.Entity("dominikz.Domain.Models.Book", b =>
                {
                    b.HasOne("dominikz.Domain.Models.Movies", null)
                        .WithOne()
                        .HasForeignKey("dominikz.Domain.Models.Book", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.Domain.Models.Game", b =>
                {
                    b.HasOne("dominikz.Domain.Models.Movies", null)
                        .WithOne()
                        .HasForeignKey("dominikz.Domain.Models.Game", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.Domain.Models.Movie", b =>
                {
                    b.HasOne("dominikz.Domain.Models.Movies", null)
                        .WithOne()
                        .HasForeignKey("dominikz.Domain.Models.Movie", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.Domain.Models.Food", b =>
                {
                    b.Navigation("Snapshots");
                });

            modelBuilder.Entity("dominikz.Domain.Models.Recipe", b =>
                {
                    b.Navigation("Ingredients");

                    b.Navigation("Steps");
                });

            modelBuilder.Entity("dominikz.Domain.Models.Song", b =>
                {
                    b.Navigation("Segments");
                });

            modelBuilder.Entity("dominikz.Domain.Models.StorageFile", b =>
                {
                    b.Navigation("Article");

                    b.Navigation("Movies");

                    b.Navigation("Recipe");
                });
#pragma warning restore 612, 618
        }
    }
}
