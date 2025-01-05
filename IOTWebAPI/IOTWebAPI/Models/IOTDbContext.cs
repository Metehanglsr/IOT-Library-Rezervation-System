using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using IOTWebAPI.Models;
using System.Threading;

namespace IOTWebAPI.Models
{
    public class IOTDbContext : DbContext
    {
        public IOTDbContext(DbContextOptions<IOTDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorValue>()
                .HasKey(sa => new { sa.SensorId, sa.ValueId });
            
            modelBuilder.Entity<Masa>()
                        .HasOne(m => m.User)
                        .WithOne(u => u.Masa)
                        .HasForeignKey<Masa>(m => m.UserId)
                        .IsRequired(false);

            modelBuilder.Entity<Masa>()
                        .HasOne(m => m.Sensor)
                        .WithOne(s => s.Masa)
                        .HasForeignKey<Sensor>(s => s.MasaId)
                        .IsRequired(false);
        }
        public DbSet<Masa> Masalar { get; set; }
        public DbSet<Sensor> Sensorler { get; set; }
        public DbSet<SensorValue> SensorValues { get; set; }
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
