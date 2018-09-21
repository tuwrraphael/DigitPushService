﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PushServer.PushConfiguration.EntityFramework;

namespace DigitPushService.Migrations
{
    [DbContext(typeof(ConfigurationDbContext))]
    partial class ConfigurationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799");

            modelBuilder.Entity("PushServer.PushConfiguration.EntityFramework.Entities.PushChannelConfiguration", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Endpoint");

                    b.Property<string>("EndpointInfo");

                    b.Property<DateTime?>("ExpirationTime");

                    b.Property<string>("Type");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("PushChannelConfigurations");
                });

            modelBuilder.Entity("PushServer.PushConfiguration.EntityFramework.Entities.PushChannelOption", b =>
                {
                    b.Property<string>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("EndpointOption");

                    b.Property<string>("Key");

                    b.Property<string>("PushChannelConfigurationID");

                    b.Property<string>("Value");

                    b.HasKey("ID");

                    b.HasIndex("PushChannelConfigurationID");

                    b.ToTable("PushChannelOptions");
                });

            modelBuilder.Entity("PushServer.PushConfiguration.EntityFramework.Entities.PushChannelOption", b =>
                {
                    b.HasOne("PushServer.PushConfiguration.EntityFramework.Entities.PushChannelConfiguration", "PushChannelConfiguration")
                        .WithMany("Options")
                        .HasForeignKey("PushChannelConfigurationID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
