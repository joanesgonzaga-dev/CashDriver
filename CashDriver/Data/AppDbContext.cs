using CashDriver.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

        public DbSet<Jornada> Jornadas => Set<Jornada>();
        //public DbSet<Ganho> Ganhos => Set<Ganho>();
        public DbSet<TipoDespesa> TiposDespesa => Set<TipoDespesa>();

        public DbSet<Ganho> Ganhos => Set<Ganho>();
        public DbSet<Despesa> Despesas => Set<Despesa>();
        public DbSet<Plataforma> Plataformas => Set<Plataforma>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Jornada>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Jornada>()
                .HasMany(j => j.Ganhos)
                .WithOne(g => g.Jornada)
                .HasForeignKey(g => g.JornadaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Jornada>()
                .HasMany(j => j.Despesas)
                .WithOne(d => d.Jornada)
                .HasForeignKey(d => d.JornadaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Despesa>()
               .HasOne(d => d.Tipo)
               .WithMany()
               .HasForeignKey(t => t.TipoDespesaId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
