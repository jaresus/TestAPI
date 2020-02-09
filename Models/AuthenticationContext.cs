using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            modelBuilder.Entity<KwalifikacjaWydzial>().HasKey(kw => new { kw.KwalifikacjaID, kw.WydzialID });

            modelBuilder.Entity<KwalifikacjaWydzial>()
                .HasOne<Kwalifikacja>(kw => kw.Kwalifikacja)
                .WithMany(s => s.KwalifikacjaWydzial)
                .HasForeignKey(kw => kw.KwalifikacjaID);


            modelBuilder.Entity<KwalifikacjaWydzial>()
                .HasOne<Wydzial>(kw => kw.Wydzial)
                .WithMany(s => s.KwalifikacjaWydzial)
                .HasForeignKey(kw => kw.WydzialID);
        }
    }
}
