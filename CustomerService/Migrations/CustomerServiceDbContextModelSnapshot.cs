﻿// <auto-generated />
using System;
using CustomerService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CustomerService.Migrations
{
    [DbContext(typeof(CustomerServiceDbContext))]
    partial class CustomerServiceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CustomerService.Models.Agent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Agents");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "markomarkovic@gmail.com",
                            FirstName = "Marko",
                            LastName = "Markovic",
                            PasswordHash = "$2a$11$b1cV4i2BoDOps/yLtkynKu67b0eyNqBEwwgootrNGV4aXXpHJYdEa"
                        },
                        new
                        {
                            Id = 2,
                            Email = "ivanivanovic@gmail.com",
                            FirstName = "Ivan",
                            LastName = "Ivanovic",
                            PasswordHash = "$2a$11$aYrwhwSP/93iMaWJnIDGgOj1UikIn.niN884ccG8Cv.3CVPE.EtqW"
                        },
                        new
                        {
                            Id = 3,
                            Email = "nikolanikolic@gmail.com",
                            FirstName = "Nikola",
                            LastName = "Nikolic",
                            PasswordHash = "$2a$11$PzFmzVj8BhmiQqr04r2bF.TKpUSLfMjT.DGwFKM2jyyh/B9TrcZpm"
                        });
                });

            modelBuilder.Entity("CustomerService.Models.Campaign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CampaignName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Campaigns");
                });

            modelBuilder.Entity("CustomerService.Models.Purchase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AgentId")
                        .HasColumnType("int");

                    b.Property<int>("CampaignId")
                        .HasColumnType("int");

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("Discount")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("PriceWithDiscount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AgentId");

                    b.HasIndex("CampaignId");

                    b.ToTable("Purchases", t =>
                        {
                            t.HasCheckConstraint("CK_Purchase_Discount", "[Discount] > 0");

                            t.HasCheckConstraint("CK_Purchase_Price", "[Price] > 0");
                        });
                });

            modelBuilder.Entity("CustomerService.Models.Purchase", b =>
                {
                    b.HasOne("CustomerService.Models.Agent", "Agent")
                        .WithMany("Purchases")
                        .HasForeignKey("AgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CustomerService.Models.Campaign", "Campaign")
                        .WithMany("Purchases")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agent");

                    b.Navigation("Campaign");
                });

            modelBuilder.Entity("CustomerService.Models.Agent", b =>
                {
                    b.Navigation("Purchases");
                });

            modelBuilder.Entity("CustomerService.Models.Campaign", b =>
                {
                    b.Navigation("Purchases");
                });
#pragma warning restore 612, 618
        }
    }
}
