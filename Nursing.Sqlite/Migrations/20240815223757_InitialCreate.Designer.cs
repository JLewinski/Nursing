﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Nursing.Services;

#nullable disable

namespace Nursing.Sqlite.Migrations
{
    [DbContext(typeof(EFDatabase))]
    [Migration("20240815223757_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("Nursing.Core.Models.DTO.FeedingDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Finished")
                        .HasColumnType("TEXT");

                    b.Property<bool>("LastIsLeft")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("LeftBreastTotal")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("RightBreastTotal")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Started")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("TotalTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Feedings");
                });
#pragma warning restore 612, 618
        }
    }
}
