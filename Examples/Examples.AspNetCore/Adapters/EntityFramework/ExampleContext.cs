using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Examples.Shared;
using Microsoft.EntityFrameworkCore;

namespace Examples.AspNetCore.Adapters.EntityFramework
{
	public class ExampleContext : DbContext
	{
		public DbSet<Board> Boards { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Pin> Pins { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Filename=./jsdata.db");
		}
	}
}
