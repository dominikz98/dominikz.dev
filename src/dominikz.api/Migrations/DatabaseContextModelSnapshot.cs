﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dominikz.api.Provider;

#nullable disable

namespace dominikz.api.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("dominikz.api.Models.Article", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<Guid>("FileId")
                        .HasColumnType("char(36)");

                    b.Property<string>("MdText")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("FileId");

                    b.ToTable("articles", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.Food", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<decimal>("Carbohydrates")
                        .HasColumnType("decimal(65,30)");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<decimal>("Fat")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("Kilocalories")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("PricePerCount")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("Protein")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("ReweUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Unit")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("foods", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.Media", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<Guid>("FileId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.ToTable("medias", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("dominikz.api.Models.MoviesPersonsMapping", b =>
                {
                    b.Property<Guid>("MovieId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("PersonId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.HasKey("MovieId", "PersonId");

                    b.HasIndex("PersonId");

                    b.ToTable("movies_persons_mapping", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.Person", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<Guid?>("FileId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.ToTable("persons", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.Recipe", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Categories")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time(6)");

                    b.Property<Guid>("FileId")
                        .HasColumnType("char(36)");

                    b.Property<string>("MdText")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Portions")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.ToTable("recipes", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.RecipesFoodsMapping", b =>
                {
                    b.Property<Guid>("RecipeId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("FoodId")
                        .HasColumnType("char(36)");

                    b.Property<decimal>("Multiplier")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("RecipeId", "FoodId");

                    b.HasIndex("FoodId");

                    b.ToTable("recipes_foods_mapping", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.Song", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("BPM")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("songs", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.SongSegment", b =>
                {
                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<Guid>("SongId")
                        .HasColumnType("char(36)");

                    b.Property<int>("BottomClef")
                        .HasColumnType("int");

                    b.Property<string>("BottomNotes")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("BottomTact")
                        .HasColumnType("int");

                    b.Property<int>("TopClef")
                        .HasColumnType("int");

                    b.Property<string>("TopNotes")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("TopTact")
                        .HasColumnType("int");

                    b.HasKey("Index", "SongId");

                    b.HasIndex("SongId");

                    b.ToTable("songs_segments", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.StorageFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<int>("Extension")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("files", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.Book", b =>
                {
                    b.HasBaseType("dominikz.api.Models.Media");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Genres")
                        .HasColumnType("int");

                    b.Property<int>("Language")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.ToTable("books", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.Game", b =>
                {
                    b.HasBaseType("dominikz.api.Models.Media");

                    b.Property<int>("Genres")
                        .HasColumnType("int");

                    b.Property<int>("Platform")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.ToTable("games", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.Movie", b =>
                {
                    b.HasBaseType("dominikz.api.Models.Media");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Genres")
                        .HasColumnType("int");

                    b.Property<string>("ImdbId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MdText")
                        .HasColumnType("longtext");

                    b.Property<string>("Plot")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Runtime")
                        .HasColumnType("time(6)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.Property<string>("YoutubeId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasIndex("AuthorId");

                    b.ToTable("movies", (string)null);
                });

            modelBuilder.Entity("dominikz.api.Models.Article", b =>
                {
                    b.HasOne("dominikz.api.Models.Person", "Author")
                        .WithMany("Articles")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dominikz.api.Models.StorageFile", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("File");
                });

            modelBuilder.Entity("dominikz.api.Models.Media", b =>
                {
                    b.HasOne("dominikz.api.Models.StorageFile", "File")
                        .WithMany("Medias")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("dominikz.api.Models.MoviesPersonsMapping", b =>
                {
                    b.HasOne("dominikz.api.Models.Movie", "Movie")
                        .WithMany("MoviesPersonsMappings")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dominikz.api.Models.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("dominikz.api.Models.Person", b =>
                {
                    b.HasOne("dominikz.api.Models.StorageFile", "File")
                        .WithMany()
                        .HasForeignKey("FileId");

                    b.Navigation("File");
                });

            modelBuilder.Entity("dominikz.api.Models.Recipe", b =>
                {
                    b.HasOne("dominikz.api.Models.StorageFile", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("dominikz.api.Models.RecipesFoodsMapping", b =>
                {
                    b.HasOne("dominikz.api.Models.Food", "Food")
                        .WithMany("RecipesFoodsMappings")
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dominikz.api.Models.Recipe", "Recipe")
                        .WithMany("RecipesFoodsMappings")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Food");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("dominikz.api.Models.SongSegment", b =>
                {
                    b.HasOne("dominikz.api.Models.Song", "Song")
                        .WithMany("Segments")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Song");
                });

            modelBuilder.Entity("dominikz.api.Models.Book", b =>
                {
                    b.HasOne("dominikz.api.Models.Media", null)
                        .WithOne()
                        .HasForeignKey("dominikz.api.Models.Book", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.api.Models.Game", b =>
                {
                    b.HasOne("dominikz.api.Models.Media", null)
                        .WithOne()
                        .HasForeignKey("dominikz.api.Models.Game", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.api.Models.Movie", b =>
                {
                    b.HasOne("dominikz.api.Models.Person", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("dominikz.api.Models.Media", null)
                        .WithOne()
                        .HasForeignKey("dominikz.api.Models.Movie", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("dominikz.api.Models.Food", b =>
                {
                    b.Navigation("RecipesFoodsMappings");
                });

            modelBuilder.Entity("dominikz.api.Models.Person", b =>
                {
                    b.Navigation("Articles");
                });

            modelBuilder.Entity("dominikz.api.Models.Recipe", b =>
                {
                    b.Navigation("RecipesFoodsMappings");
                });

            modelBuilder.Entity("dominikz.api.Models.Song", b =>
                {
                    b.Navigation("Segments");
                });

            modelBuilder.Entity("dominikz.api.Models.StorageFile", b =>
                {
                    b.Navigation("Medias");
                });

            modelBuilder.Entity("dominikz.api.Models.Movie", b =>
                {
                    b.Navigation("MoviesPersonsMappings");
                });
#pragma warning restore 612, 618
        }
    }
}
