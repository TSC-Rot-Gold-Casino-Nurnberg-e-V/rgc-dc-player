﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TournamentDJ.Model;

#nullable disable

namespace TournamentDJ.Migrations
{
    [DbContext(typeof(TrackContext))]
    [Migration("20250321143024_AddingUpdateTimestamp")]
    partial class AddingUpdateTimestamp
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("TournamentDJ.Model.Dance", b =>
                {
                    b.Property<int>("DanceTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.PrimitiveCollection<string>("DanceIdentifiers")
                        .HasColumnType("TEXT");

                    b.Property<int>("MaxBPM")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinBPM")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("DanceTypeId");

                    b.ToTable("Dances");
                });

            modelBuilder.Entity("TournamentDJ.Model.DanceRound", b =>
                {
                    b.Property<int>("DanceRoundId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxDifficulty")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinCharacteristics")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinDifficulty")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("DanceRoundId");

                    b.ToTable("DanceRounds");
                });

            modelBuilder.Entity("TournamentDJ.Model.OrderElement<TournamentDJ.Model.Dance>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DanceRoundId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ObjectToOrderDanceTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OrderRank")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DanceRoundId");

                    b.HasIndex("ObjectToOrderDanceTypeId");

                    b.ToTable("OrderElementsDance");
                });

            modelBuilder.Entity("TournamentDJ.Model.Tag", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Color")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TrackId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Name");

                    b.HasIndex("TrackId");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("TournamentDJ.Model.Track", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Album")
                        .HasColumnType("TEXT");

                    b.Property<uint?>("BeatsPerMinute")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Characteristic")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<int?>("DanceTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Difficulty")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("TEXT");

                    b.Property<bool>("FlaggedAsFavourite")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("FlaggedForReview")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Genre")
                        .HasColumnType("TEXT");

                    b.Property<string>("ISRC")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastDataUpdateTimestamp")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastPlayedTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("Uris")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DanceTypeId");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("TournamentDJ.Model.TrackList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TrackLists");
                });

            modelBuilder.Entity("TrackTrackList", b =>
                {
                    b.Property<int>("TrackListsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TracksId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TrackListsId", "TracksId");

                    b.HasIndex("TracksId");

                    b.ToTable("TrackTrackList");
                });

            modelBuilder.Entity("TournamentDJ.Model.OrderElement<TournamentDJ.Model.Dance>", b =>
                {
                    b.HasOne("TournamentDJ.Model.DanceRound", null)
                        .WithMany("OrderElements")
                        .HasForeignKey("DanceRoundId");

                    b.HasOne("TournamentDJ.Model.Dance", "ObjectToOrder")
                        .WithMany()
                        .HasForeignKey("ObjectToOrderDanceTypeId");

                    b.Navigation("ObjectToOrder");
                });

            modelBuilder.Entity("TournamentDJ.Model.Tag", b =>
                {
                    b.HasOne("TournamentDJ.Model.Track", null)
                        .WithMany("Tags")
                        .HasForeignKey("TrackId");
                });

            modelBuilder.Entity("TournamentDJ.Model.Track", b =>
                {
                    b.HasOne("TournamentDJ.Model.Dance", "Dance")
                        .WithMany("Tracks")
                        .HasForeignKey("DanceTypeId");

                    b.Navigation("Dance");
                });

            modelBuilder.Entity("TrackTrackList", b =>
                {
                    b.HasOne("TournamentDJ.Model.TrackList", null)
                        .WithMany()
                        .HasForeignKey("TrackListsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TournamentDJ.Model.Track", null)
                        .WithMany()
                        .HasForeignKey("TracksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TournamentDJ.Model.Dance", b =>
                {
                    b.Navigation("Tracks");
                });

            modelBuilder.Entity("TournamentDJ.Model.DanceRound", b =>
                {
                    b.Navigation("OrderElements");
                });

            modelBuilder.Entity("TournamentDJ.Model.Track", b =>
                {
                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
