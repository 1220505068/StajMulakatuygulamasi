using Microsoft.EntityFrameworkCore;
using StajMulakatuygulamasi.Models;
using System;
using System.Collections.Generic;

namespace StajMulakatuygulaması.Models;

public partial class StajMulakatDbContext : DbContext
{

    public StajMulakatDbContext(DbContextOptions<StajMulakatDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<HocaMusaitlik> HocaMusaitlik { get; set; }
    public virtual DbSet<OgrenciMesguliyet> OgrenciMesguliyet { get; set; }

    public DbSet<DonemAyari> DonemAyarlari { get; set; }
    public DbSet<OgrenciMesguliyet> OgrenciMesguliyetleri { get; set; }
    public DbSet<HocaMusaitlik> HocaMusaitliklar { get; set; }
    public DbSet<MulakatAtama> MulakatAtamalari { get; set; }

    public virtual DbSet<Kullanicilar> Kullanicilar { get; set; }

    public virtual DbSet<OgrenciUygunluk> OgrenciUygunluk { get; set; }

    public virtual DbSet<PlanlananMulakatlar> PlanlananMulakatlar { get; set; }

    public virtual DbSet<Roller> Rollers { get; set; }
    public DbSet<Bildirim> Bildirimler { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=StajMulakatDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HocaMusaitlik>(entity =>
        {
            // Birincil Anahtar (Primary Key)
            entity.HasKey(e => e.MusaitlikID);

            // Tablo Adı
            entity.ToTable("HocaMusaitlik");

            // Sütun Yapılandırmaları
            entity.Property(e => e.MusaitlikID).HasColumnName("MusaitlikID");
            entity.Property(e => e.KullaniciID).HasColumnName("KullaniciID");
            entity.Property(e => e.Gun).HasColumnName("Gun").IsRequired();
            entity.Property(e => e.Saat).HasColumnName("Saat").IsRequired();

            // İlişki (Yabancı Anahtar)
            entity.HasOne(d => d.Hoca)
            .WithMany(p => p.HocaMusaitliklari) 
            .HasForeignKey(d => d.KullaniciID)
            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Kullanic__1788CCAC8A426844");

            entity.ToTable("Kullanicilar");

            entity.HasIndex(e => e.Username, "UQ__Kullanic__536C85E41B42F5C8").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Ad).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Soyad).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Kullanicilars)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Kullanici__RoleI__4E88ABD4");
        });

        modelBuilder.Entity<OgrenciUygunluk>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PK__OgrenciU__DA3979918F4C9BD9");

            entity.ToTable("OgrenciUygunluk");

            entity.Property(e => e.AvailabilityId).HasColumnName("AvailabilityID");
            entity.Property(e => e.BaslangicZamani).HasColumnType("datetime");
            entity.Property(e => e.BitisZamani).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.Tip).HasMaxLength(20);

            entity.HasOne(d => d.Student).WithMany(p => p.OgrenciUygunluks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OgrenciUy__Stude__5165187F");
        });

        modelBuilder.Entity<PlanlananMulakatlar>(entity =>
        {
            entity.HasKey(e => e.MulakatId).HasName("PK__Planlana__B5BFD791D86F1B40");

            entity.ToTable("PlanlananMulakatlar");

            entity.Property(e => e.MulakatId).HasColumnName("MulakatID");
            entity.Property(e => e.Durum)
                .HasMaxLength(20)
                .HasDefaultValue("Beklemede");
            entity.Property(e => e.HocaId).HasColumnName("HocaID");
            entity.Property(e => e.BaslangicSaati).HasColumnType("datetime");
            entity.Property(e => e.BitisSaati).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Hoca).WithMany(p => p.PlanlananMulakatlarHocas)
                .HasForeignKey(d => d.HocaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Planlanan__HocaI__59063A47");

            entity.HasOne(d => d.Student).WithMany(p => p.PlanlananMulakatlarStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Planlanan__Stude__5812160E");
        });

        modelBuilder.Entity<Roller>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roller__8AFACE3A2F18EB87");

            entity.ToTable("Roller");

            entity.HasIndex(e => e.RoleName, "UQ__Roller__8A2B61604C431A96").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
