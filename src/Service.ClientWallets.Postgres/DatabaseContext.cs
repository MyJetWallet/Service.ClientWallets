using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service.ClientWallets.Postgres
{
    public class DatabaseContext : DbContext
    {
        public const string Schema = "clientwallets";

        public const string TableName = "wallets";

        public DbSet<ClientWalletEntity> ClientWallet { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public static ILoggerFactory LoggerFactory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<ClientWalletEntity>().ToTable(TableName);
            modelBuilder.Entity<ClientWalletEntity>().HasKey(e => e.WalletId);
            
            modelBuilder.Entity<ClientWalletEntity>().HasIndex(e => new {e.BrokerId, e.ClientId});

            modelBuilder.Entity<ClientWalletEntity>().Property(e => e.BrandId).HasMaxLength(128);
            modelBuilder.Entity<ClientWalletEntity>().Property(e => e.WalletId).HasMaxLength(128);
            modelBuilder.Entity<ClientWalletEntity>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<ClientWalletEntity>().Property(e => e.BrokerId).HasMaxLength(128);
            modelBuilder.Entity<ClientWalletEntity>().Property(e => e.Name).HasMaxLength(1024*3);

            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> UpsetAsync(IEnumerable<ClientWalletEntity> entities)
        {
            var result = await ClientWallet.UpsertRange(entities).On(e => e.WalletId).NoUpdate().RunAsync();
            return result;
        }

        
    }
}
