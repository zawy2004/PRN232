using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Q1.Models;

public partial class Prn232PeSu2611Context : DbContext
{
    public Prn232PeSu2611Context()
    {
    }

    public Prn232PeSu2611Context(DbContextOptions<Prn232PeSu2611Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketDetail> TicketDetails { get; set; }

    public virtual DbSet<Viewer> Viewers { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.ComboId).HasName("PK__Combos__DD42580E9C178533");

            entity.Property(e => e.ComboId).HasColumnName("ComboID");
            entity.Property(e => e.ComboName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__0385055ECFEC1028");

            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.GenreName).HasMaxLength(100);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__4BD2943A491B1C7F");

            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.BasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasMany(d => d.Genres).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MovieGenr__Genre__46E78A0C"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MovieGenr__Movie__45F365D3"),
                    j =>
                    {
                        j.HasKey("MovieId", "GenreId").HasName("PK__MovieGen__BBEAC46F4D57A8D3");
                        j.ToTable("MovieGenres");
                        j.IndexerProperty<int>("MovieId").HasColumnName("MovieID");
                        j.IndexerProperty<int>("GenreId").HasColumnName("GenreID");
                    });
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickets__712CC62746FE66B8");

            entity.Property(e => e.TicketId).HasColumnName("TicketID");
            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.ShowTime).HasColumnType("datetime");
            entity.Property(e => e.ViewerId).HasColumnName("ViewerID");

            entity.HasOne(d => d.Viewer).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ViewerId)
                .HasConstraintName("FK__Tickets__ViewerI__3F466844");
        });

        modelBuilder.Entity<TicketDetail>(entity =>
        {
            entity.HasKey(e => new { e.TicketId, e.MovieId }).HasName("PK__TicketDe__C591EF6469532FB4");

            entity.Property(e => e.TicketId).HasColumnName("TicketID");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.SeatNumber).HasMaxLength(10);

            entity.HasOne(d => d.Movie).WithMany(p => p.TicketDetails)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TicketDet__Movie__4316F928");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketDetails)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TicketDet__Ticke__4222D4EF");
        });

        modelBuilder.Entity<Viewer>(entity =>
        {
            entity.HasKey(e => e.ViewerId).HasName("PK__Viewers__6DA5552DD28C85BB");

            entity.Property(e => e.ViewerId).HasColumnName("ViewerID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
