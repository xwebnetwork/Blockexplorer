using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blockexplorer.Entities
{
    // Get-Help about_EntityFrameworkCore
    //
    public class ObsidianChainContext : DbContext
    {
        static string _connectionString;
        public DbSet<BlockEntity> BlockEntities { get; set; }
        public DbSet<TransactionEntity> TransactionEntities { get; set; }
        public DbSet<AddressEntity> AddressEntities { get; set; }

        public ObsidianChainContext() { }

        public ObsidianChainContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString == null)
            {
                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                var directory = Path.GetDirectoryName(location);
                var csPath = Path.Combine(directory, "connectionstring.secret");
                if (File.Exists(csPath))
                    _connectionString = File.ReadAllText(csPath).Trim();
                else
                {
                    // if the file does not exists, we are probably running ef tools
                    string pathForEfTools = @"C:\NObsidian\Blockexplorer\Blockexplorer.Entities\connectionstring.secret";
                    _connectionString = File.ReadAllText(pathForEfTools).Trim();
                }
            }

            // optionsBuilder.UseSqlServer(_connectionString, opt => opt.EnableRetryOnFailure());
            optionsBuilder.UseSqlServer(_connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlockEntity>()
                .HasIndex(p => new { p.BlockHash }).ForSqlServerIsClustered(false).IsUnique();
        }
    }

    public class BlockEntity
    {
        /// <summary>
        /// Blocknumber, starting with 1, Height = 0
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int Height { get; set; }
        [MaxLength(64)]
        public string BlockHash { get; set; }
        public List<TransactionEntity> Transactions { get; set; }
        public string BlockData { get; set; }
    }

    public class TransactionEntity
    {
        [MaxLength(64)]
        public string Id { get; set; }
        public BlockEntity BlockEntity { get; set; }
        public string TransactionData { get; set; }
    }



    public class AddressEntity
    {
        [MaxLength(34)]
        public string Id { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Balance { get; set; }
        public string TxIdBlob { get; set; }

        public int LastModifiedBlockHeight { get; set; }
    }
}
