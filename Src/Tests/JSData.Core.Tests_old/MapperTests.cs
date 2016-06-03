using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JSData.Core.Tests
{
	// This project can output the Class library as a NuGet Package.
	// To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
	[TestFixture]
	public class MapperTests
	{
		public MapperTests()
		{
		}

		[Test]
		public void StoreIsWorking()
		{
			var expectedBoards = CreateBoards(10);
			var cache = new Dictionary<string, IEnumerable<object>> { ["board"] = expectedBoards };
			var adapter = new EntityFrameworkAdapter(cache);
			var store = new DataStore(adapter);
			var actual = store.FindAll("board");
			Assert.AreEqual(expectedBoards, actual);
		}

		public IEnumerable<Board> CreateBoards(int number)
		{
			for (var i = 1; i <= number; i++)
			{
				yield return new Board()
				{
					Id = i,
					Description = "This is the #" + i + " board in America.",
					Title = "Board #" + i,
				};
			}
		}
	}
}
