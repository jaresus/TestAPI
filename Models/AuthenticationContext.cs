using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TestAPI.Models.Ocena;
using TestAPI.Models;

namespace TestAPI.Models
{
    public class AuthenticationContext : IdentityDbContext
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Ocena> Oceny { get; set; }
        public DbSet<Kwalifikacja> Kwalifikacje { get; set; }
        public DbSet<Pracownik> Pracownicy { get; set; }
        public DbSet<Wydzial> Wydzialy { get; set; }
        public DbSet<KwalifikacjaWydzial> KwalifikacjeWydzialy { get; set; }
        public DbSet<PoczatkoweWydzialy> PoczatkoweWydzialy { get; set; }
        public DbSet<OcenaArchiwum> OcenaArchiwum { get; set; }
        public DbSet<Stanowisko> Stanowiska { get; set; }
        public DbSet<KwalifikacjaStanowisko> KwalifikacjeStanowiska { get; set; }
        public DbSet<PoczatkoweStanowiska> PoczatkoweStanowiska { get; set; }
        public DbSet<SzkolenieCel> SzkolenieCele { get; set; }
        public AuthenticationContext(DbContextOptions options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Ocena>()
                  .HasOne(p => p.Pracownik)
                  .WithMany(b => b.Oceny)
                  .OnDelete(DeleteBehavior.Restrict);

            #region tabela bez klucza głównego - KwalifikacjaWydzial -
            modelBuilder.Entity<KwalifikacjaWydzial>().HasKey(kw => new { kw.KwalifikacjaID, kw.WydzialID });
            modelBuilder.Entity<KwalifikacjaWydzial>()
                .HasOne<Kwalifikacja>(kw => kw.Kwalifikacja)
                .WithMany(s => s.KwalifikacjaWydzial)
                .HasForeignKey(kw => kw.KwalifikacjaID);
            modelBuilder.Entity<KwalifikacjaWydzial>()
                .HasOne<Wydzial>(kw => kw.Wydzial)
                .WithMany(s => s.KwalifikacjaWydzial)
                .HasForeignKey(kw => kw.WydzialID);
            #endregion

            #region tabela bez klucza głównego - KwalifikacjaStanowisko -
            modelBuilder.Entity<KwalifikacjaStanowisko>().HasKey(kw => new { kw.KwalifikacjaID, kw.StanowiskoID });
            modelBuilder.Entity<KwalifikacjaStanowisko>()
                .HasOne<Kwalifikacja>(kw => kw.Kwalifikacja)
                .WithMany(s => s.KwalifikacjaStanowisko)
                .HasForeignKey(kw => kw.KwalifikacjaID);
            modelBuilder.Entity<KwalifikacjaStanowisko>()
                .HasOne<Stanowisko>(kw => kw.Stanowisko)
                .WithMany(s => s.KwalifikacjaStanowisko)
                .HasForeignKey(kw => kw.StanowiskoID);
            #endregion        

            #region debian MariaDB
            // Shorten key length for Identity
            // gdy nie skrócone rozmiary pól to powstawał błąd na debian mariadb
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(85));
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(85));

            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(85));

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasMaxLength(85);
                entity.Property(m => m.ProviderKey).HasMaxLength(85);
                entity.Property(m => m.UserId).HasMaxLength(85);
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasMaxLength(85);
                entity.Property(m => m.RoleId).HasMaxLength(85);
            });
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasMaxLength(85);
                entity.Property(m => m.LoginProvider).HasMaxLength(85);
                entity.Property(m => m.Name).HasMaxLength(85);
            });
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.Property(m => m.Id).HasMaxLength(85);
                entity.Property(m => m.UserId).HasMaxLength(85);
            });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => {
                entity.Property(m => m.Id).HasMaxLength(85);
                entity.Property(m => m.RoleId).HasMaxLength(85);
            });
            #endregion
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
        //}
    }
}
