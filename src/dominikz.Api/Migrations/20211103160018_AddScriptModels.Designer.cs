﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dominikz.Api;

namespace dominikz.Api.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20211103160018_AddScriptModels")]
    partial class AddScriptModels
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.11");

            modelBuilder.Entity("ActivityItemTag", b =>
                {
                    b.Property<int>("ActivitiesId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TagsName")
                        .HasColumnType("TEXT");

                    b.HasKey("ActivitiesId", "TagsName");

                    b.HasIndex("TagsName");

                    b.ToTable("ActivityItemTag");
                });

            modelBuilder.Entity("MovieMovieStar", b =>
                {
                    b.Property<int>("MoviesId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StarsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("MoviesId", "StarsId");

                    b.HasIndex("StarsId");

                    b.ToTable("MovieMovieStar");
                });

            modelBuilder.Entity("dominikz.Api.Models.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Release")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("activities");
                });

            modelBuilder.Entity("dominikz.Api.Models.ItemTag", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Name");

                    b.ToTable("tags");
                });

            modelBuilder.Entity("dominikz.Api.Models.MovieRating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Actors")
                        .HasMaxLength(5)
                        .HasColumnType("INTEGER");

                    b.Property<int>("Ambience")
                        .HasMaxLength(5)
                        .HasColumnType("INTEGER");

                    b.Property<int>("Music")
                        .HasMaxLength(5)
                        .HasColumnType("INTEGER");

                    b.Property<int>("Plot")
                        .HasMaxLength(5)
                        .HasColumnType("INTEGER");

                    b.Property<int>("Regie")
                        .HasMaxLength(5)
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("movie_ratings");
                });

            modelBuilder.Entity("dominikz.Api.Models.MovieStar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Job")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfilePictureUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Surename")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("movie_starts");
                });

            modelBuilder.Entity("dominikz.Api.Models.Blogpost", b =>
                {
                    b.HasBaseType("dominikz.Api.Models.Activity");

                    b.Property<string>("Banner")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MDContent")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.ToTable("blogposts");
                });

            modelBuilder.Entity("dominikz.Api.Models.Movie", b =>
                {
                    b.HasBaseType("dominikz.Api.Models.Activity");

                    b.Property<string>("KeyWord")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MDContent")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Provider")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Publication")
                        .HasColumnType("TEXT");

                    b.Property<int>("RatingId")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("Runtime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Thumbnail")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("USK")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Watched")
                        .HasColumnType("TEXT");

                    b.Property<string>("YoutubeTrailerId")
                        .HasColumnType("TEXT");

                    b.HasIndex("RatingId")
                        .IsUnique();

                    b.ToTable("movies");
                });

            modelBuilder.Entity("dominikz.Api.Models.PenAndPaper", b =>
                {
                    b.HasBaseType("dominikz.Api.Models.Activity");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MDContent")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.ToTable("penandpaper");
                });

            modelBuilder.Entity("dominikz.Api.Models.Script", b =>
                {
                    b.HasBaseType("dominikz.Api.Models.Activity");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.ToTable("scripts");
                });

            modelBuilder.Entity("ActivityItemTag", b =>
                {
                    b.HasOne("dominikz.Api.Models.Activity", null)
                        .WithMany()
                        .HasForeignKey("ActivitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dominikz.Api.Models.ItemTag", null)
                        .WithMany()
                        .HasForeignKey("TagsName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MovieMovieStar", b =>
                {
                    b.HasOne("dominikz.Api.Models.Movie", null)
                        .WithMany()
                        .HasForeignKey("MoviesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dominikz.Api.Models.MovieStar", null)
                        .WithMany()
                        .HasForeignKey("StarsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.Api.Models.Blogpost", b =>
                {
                    b.HasOne("dominikz.Api.Models.Activity", null)
                        .WithOne()
                        .HasForeignKey("dominikz.Api.Models.Blogpost", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.Api.Models.Movie", b =>
                {
                    b.HasOne("dominikz.Api.Models.Activity", null)
                        .WithOne()
                        .HasForeignKey("dominikz.Api.Models.Movie", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dominikz.Api.Models.MovieRating", "Rating")
                        .WithOne("Movie")
                        .HasForeignKey("dominikz.Api.Models.Movie", "RatingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rating");
                });

            modelBuilder.Entity("dominikz.Api.Models.PenAndPaper", b =>
                {
                    b.HasOne("dominikz.Api.Models.Activity", null)
                        .WithOne()
                        .HasForeignKey("dominikz.Api.Models.PenAndPaper", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.Api.Models.Script", b =>
                {
                    b.HasOne("dominikz.Api.Models.Activity", null)
                        .WithOne()
                        .HasForeignKey("dominikz.Api.Models.Script", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("dominikz.Api.Models.MovieRating", b =>
                {
                    b.Navigation("Movie");
                });
#pragma warning restore 612, 618
        }
    }
}
