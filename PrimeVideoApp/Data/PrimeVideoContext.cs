using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PrimeVideoApp.Models;

namespace PrimeVideoApp.Data;

public partial class PrimeVideoContext : DbContext
{
    public PrimeVideoContext(DbContextOptions<PrimeVideoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Content> Contents { get; set; }

    public virtual DbSet<ContentPerson> ContentPeople { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<VwContentSummary> VwContentSummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("content_pkey");

            entity.ToTable("content");

            entity.HasIndex(e => e.SourceId, "content_source_id_key").IsUnique();

            entity.HasIndex(e => e.Title, "idx_content_title");

            entity.HasIndex(e => e.ContentType, "idx_content_type");

            entity.HasIndex(e => e.ReleaseYear, "idx_content_year");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ContentType)
                .HasMaxLength(20)
                .HasColumnName("content_type");
            entity.Property(e => e.DateAdded).HasColumnName("date_added");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationText)
                .HasMaxLength(50)
                .HasColumnName("duration_text");
            entity.Property(e => e.DurationUnit)
                .HasMaxLength(20)
                .HasColumnName("duration_unit");
            entity.Property(e => e.DurationValue).HasColumnName("duration_value");
            entity.Property(e => e.Rating)
                .HasMaxLength(30)
                .HasColumnName("rating");
            entity.Property(e => e.ReleaseYear).HasColumnName("release_year");
            entity.Property(e => e.SourceId)
                .HasMaxLength(20)
                .HasColumnName("source_id");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");

            entity.HasMany(d => d.Countries).WithMany(p => p.Contents)
                .UsingEntity<Dictionary<string, object>>(
                    "ContentCountry",
                    r => r.HasOne<Country>().WithMany()
                        .HasForeignKey("CountryId")
                        .HasConstraintName("fk_cc_country"),
                    l => l.HasOne<Content>().WithMany()
                        .HasForeignKey("ContentId")
                        .HasConstraintName("fk_cc_content"),
                    j =>
                    {
                        j.HasKey("ContentId", "CountryId").HasName("content_country_pkey");
                        j.ToTable("content_country");
                        j.HasIndex(new[] { "CountryId" }, "idx_cc_country");
                        j.IndexerProperty<int>("ContentId").HasColumnName("content_id");
                        j.IndexerProperty<int>("CountryId").HasColumnName("country_id");
                    });

            entity.HasMany(d => d.Genres).WithMany(p => p.Contents)
                .UsingEntity<Dictionary<string, object>>(
                    "ContentGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("fk_cg_genre"),
                    l => l.HasOne<Content>().WithMany()
                        .HasForeignKey("ContentId")
                        .HasConstraintName("fk_cg_content"),
                    j =>
                    {
                        j.HasKey("ContentId", "GenreId").HasName("content_genre_pkey");
                        j.ToTable("content_genre");
                        j.HasIndex(new[] { "GenreId" }, "idx_cg_genre");
                        j.IndexerProperty<int>("ContentId").HasColumnName("content_id");
                        j.IndexerProperty<int>("GenreId").HasColumnName("genre_id");
                    });
        });

        modelBuilder.Entity<ContentPerson>(entity =>
        {
            entity.HasKey(e => new { e.ContentId, e.PersonId, e.Role }).HasName("content_person_pkey");

            entity.ToTable("content_person");

            entity.HasIndex(e => e.PersonId, "idx_cp_person");

            entity.Property(e => e.ContentId).HasColumnName("content_id");
            entity.Property(e => e.PersonId).HasColumnName("person_id");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.CreditOrder).HasColumnName("credit_order");

            entity.HasOne(d => d.Content).WithMany(p => p.ContentPeople)
                .HasForeignKey(d => d.ContentId)
                .HasConstraintName("fk_cp_content");

            entity.HasOne(d => d.Person).WithMany(p => p.ContentPeople)
                .HasForeignKey(d => d.PersonId)
                .HasConstraintName("fk_cp_person");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("country_pkey");

            entity.ToTable("country");

            entity.HasIndex(e => e.Name, "country_name_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genre_pkey");

            entity.ToTable("genre");

            entity.HasIndex(e => e.Name, "genre_name_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("person_pkey");

            entity.ToTable("person");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<VwContentSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_content_summary");

            entity.Property(e => e.CastCount).HasColumnName("cast_count");
            entity.Property(e => e.ContentType)
                .HasMaxLength(20)
                .HasColumnName("content_type");
            entity.Property(e => e.DirectorCount).HasColumnName("director_count");
            entity.Property(e => e.DurationUnit)
                .HasMaxLength(20)
                .HasColumnName("duration_unit");
            entity.Property(e => e.DurationValue).HasColumnName("duration_value");
            entity.Property(e => e.GenreCount).HasColumnName("genre_count");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rating)
                .HasMaxLength(30)
                .HasColumnName("rating");
            entity.Property(e => e.ReleaseYear).HasColumnName("release_year");
            entity.Property(e => e.SourceId)
                .HasMaxLength(20)
                .HasColumnName("source_id");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
