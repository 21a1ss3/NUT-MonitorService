using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.DB.UpsStatus
{
    public class UPSMonDBModel : DbContext
    {
        private static Sensetive.DB.UpsMonDbconnectionString _connStringHolder = new Sensetive.DB.UpsMonDbconnectionString();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ////For Update-Database
            //OracleConfiguration.TnsAdmin = System.IO.Path.Combine(AppContext.BaseDirectory, "Sensetive/DB/Wallet");
            //OracleConfiguration.WalletLocation = System.IO.Path.Combine(AppContext.BaseDirectory, "Sensetive/DB/Wallet");
            optionsBuilder.UseOracle(_connStringHolder.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UpsMesurement>().HasKey(t => new { t.SessionID, t.Timestamp });
            modelBuilder.Entity<UpsMesurement>().HasOne(m => m.MonitorSession).WithMany(s => s.UpsMesurements).HasForeignKey(m => m.SessionID);
            modelBuilder.Entity<UpsMesurement>().HasOne(m => m.StatusDescription).WithMany(s => s.UpsMesurements).HasForeignKey(m => m.StatusID);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<MonitorSession> MonitorSessions { get; set; }
        public DbSet<UpsMesurement> UpsMesurements { get; set; }
        public DbSet<UpsStatus> UpsStatuses { get; set; }
    }
}
