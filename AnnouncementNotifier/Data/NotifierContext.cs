using AnnouncementNotifier.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace AnnouncementNotifier.Data
{
    class NotifierContext : DbContext
    {
        public DbSet<NotifyHistory> NotifyHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(ConfigurationManager.AppSettings["DbConnectionString"]);
        }
    }
}
