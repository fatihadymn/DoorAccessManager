﻿// <auto-generated />
using System;
using DoorAccessManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DoorAccessManager.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240502205046_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.18");

            modelBuilder.Entity("DoorAccessManager.Items.Entities.Door", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OfficeId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OfficeId");

                    b.ToTable("Doors");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.DoorAccessLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("DoorId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSuccess")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DoorId");

                    b.HasIndex("UserId");

                    b.ToTable("DoorAccessLogs");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.DoorRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("DoorId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DoorId");

                    b.HasIndex("RoleId");

                    b.ToTable("DoorRoles");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.Office", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Offices");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OfficeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OfficeId");

                    b.HasIndex("RoleId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.Door", b =>
                {
                    b.HasOne("DoorAccessManager.Items.Entities.Office", "Office")
                        .WithMany("Doors")
                        .HasForeignKey("OfficeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Office");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.DoorAccessLog", b =>
                {
                    b.HasOne("DoorAccessManager.Items.Entities.Door", "Door")
                        .WithMany("DoorAccessLogs")
                        .HasForeignKey("DoorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DoorAccessManager.Items.Entities.User", "User")
                        .WithMany("DoorAccessLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Door");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.DoorRole", b =>
                {
                    b.HasOne("DoorAccessManager.Items.Entities.Door", "Door")
                        .WithMany("DoorRoles")
                        .HasForeignKey("DoorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DoorAccessManager.Items.Entities.Role", "Role")
                        .WithMany("DoorRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Door");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.User", b =>
                {
                    b.HasOne("DoorAccessManager.Items.Entities.Office", "Office")
                        .WithMany("Users")
                        .HasForeignKey("OfficeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DoorAccessManager.Items.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Office");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.Door", b =>
                {
                    b.Navigation("DoorAccessLogs");

                    b.Navigation("DoorRoles");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.Office", b =>
                {
                    b.Navigation("Doors");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.Role", b =>
                {
                    b.Navigation("DoorRoles");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("DoorAccessManager.Items.Entities.User", b =>
                {
                    b.Navigation("DoorAccessLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
