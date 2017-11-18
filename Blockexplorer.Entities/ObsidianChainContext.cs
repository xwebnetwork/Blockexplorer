using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Blockexplorer.Entities
{
	// Get-Help about_EntityFrameworkCore
	//
	public class ObsidianChainContext : DbContext
	{
		static string _connectionString;
		public DbSet<BlockEntity> Blocks { get; set; }
		public DbSet<TransactionEntity> Transactions { get; set; }
		public DbSet<TransactionAddressEntity> TransactionAddresses { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if(_connectionString == null)
			{
				var location = System.Reflection.Assembly.GetEntryAssembly().Location;
				var directory = Path.GetDirectoryName(location);
				var csPath = Path.Combine(directory, "connectionstring.secret");
				_connectionString = File.ReadAllText(csPath);
			}
				
			optionsBuilder.UseSqlServer(_connectionString);
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TransactionAddressEntity>()
				.HasIndex(p => new { p.Address }).ForSqlServerIsClustered(false).IsUnique(false);
		}
	}

	public class BlockEntity
	{
		public Guid Id { get; set; }
		public int Height { get; set; }
		public string BlockHash { get; set; }
		public List<TransactionEntity> Transactions { get; set; }
		public string BlockData { get; set; }
	}

	public class TransactionEntity
	{
		public string Id { get; set; }
		public BlockEntity BlockEntity { get; set; }
		public string TransactionData { get; set; }
		public List<TransactionAddressEntity> TransactionAddresses { get; set; }
	}

	public class TransactionAddressEntity
	{
		public int Id { get; set; }
		public TransactionEntity TransactionEntity { get; set; }
		public string TransactionEntityId { get; set; }

		[MaxLength(34)]
		public string Address { get; set; }
	}
}
