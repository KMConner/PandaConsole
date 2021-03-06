﻿// <auto-generated />
using System;
using AnnouncementNotifier.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AnnouncementNotifier.Migrations
{
    [DbContext(typeof(NotifierContext))]
    partial class NotifierContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AnnouncementNotifier.Models.NotifyHistory", b =>
                {
                    b.Property<Guid>("AnnouncementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("AnnouncementId");

                    b.ToTable("NotifyHistory");
                });
#pragma warning restore 612, 618
        }
    }
}
