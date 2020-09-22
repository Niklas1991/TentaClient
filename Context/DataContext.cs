using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using TentaClient.Model.Entities;

namespace TentaClient.DataContext
{
	public class DataContext : DbContext
	{
		private const string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TentaClient;Integrated Security=True;";

		protected override void OnConfiguring(DbContextOptionsBuilder contextOptionsBuilder)
		{

			if (!contextOptionsBuilder.IsConfigured)
			{
				contextOptionsBuilder.UseSqlServer(connectionString);
			}
		}
		public virtual DbSet<Account> Accounts { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
