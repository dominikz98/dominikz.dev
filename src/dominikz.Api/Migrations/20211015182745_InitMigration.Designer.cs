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
    [Migration("20211015182745_InitMigration")]
    partial class InitMigration
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

            modelBuilder.Entity("dominikz.Api.Models.Blogpost", b =>
                {
                    b.HasOne("dominikz.Api.Models.Activity", null)
                        .WithOne()
                        .HasForeignKey("dominikz.Api.Models.Blogpost", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
