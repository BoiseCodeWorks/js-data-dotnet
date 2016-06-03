using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Linq.Dynamic;
using JsData;
using Newtonsoft.Json;

namespace JSData.Tests
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
		public void FindAllReturnsAll()
		{

			var expectedBoards = CreateBoards(10).ToList();
			var cache = new Dictionary<string, IEnumerable<object>> { ["board"] = expectedBoards };
			var adapter = new InMemoryAdapter(cache);
			var store = new DataStore(adapter);
			store.Map<Board>("board");
			var actualBoards = store.FindAll("board");
			Assert.AreEqual(expectedBoards, actualBoards);
		}

		[Test]
		public void FindAllWithQueryReturnsAll()
		{
			var json = @"
				{
					""where"": {
						""Id"": {
							""=="": 12,
							""|>="": 17
						}
					}
				}";
			var query = JsonConvert.DeserializeObject<JsQuery>(json);
			var options = new JsOptions() { Query = query };
			var expectedBoards = CreateBoards(10).ToList();
			var cache = new Dictionary<string, IEnumerable<object>> { ["board"] = expectedBoards };
			var adapter = new InMemoryAdapter(cache);
			var store = new DataStore(adapter);
			store.Map<Board>("board");
			var actualBoards = store.FindAll("board", options).ToList();
			Assert.AreEqual(expectedBoards.Where(x => x.Id == 12 || x.Id >= 17), actualBoards);
		}


		[Test]
		public void FindReturnsCorrectItem()
		{

			var expectedBoards = CreateBoards(10).ToList();
			var cache = new Dictionary<string, IEnumerable<object>> { ["board"] = expectedBoards };
			var adapter = new InMemoryAdapter(cache);
			var store = new DataStore(adapter);
			store.Map<Board>("board");

			var expectedBoard = expectedBoards[4];
			var actualBoard = store.Find("board", expectedBoard.Id);
			//var where = new Interpreter().ParseAsExpression<Func<Board, bool>>("board.Id == 5", "board");
			//var actualBoard = store.FindAll("board").Where(JsQuery.Ops["=="]("Id", expectedBoard.Id)).FirstOrDefault();

			Assert.AreEqual(expectedBoard, actualBoard);
		}


		[Test]
		public void CreateWorks()
		{

			var expectedBoards = CreateBoards(10).ToList();
			var cache = new Dictionary<string, IEnumerable<object>> { ["board"] = expectedBoards };
			var adapter = new InMemoryAdapter(cache);
			var store = new DataStore(adapter);
			store.Map<Board>("board");

			var board = new Board
			{
				Id = 780,
				Title = "A new Board",
				Description = "Cool Description",
				GroupId = 1234,
				Colors = new List<string>() { "Red" }
			};

			var createdBoard = store.Create("board", board);

			var actualBoard = store.Find("board", 780);

			Assert.AreSame(actualBoard, createdBoard);

		}

		[Test]
		public void CreateNonGenericWorks()
		{

			var expectedBoards = CreateBoards(10).ToList();
			var cache = new Dictionary<string, IEnumerable<object>> { ["board"] = expectedBoards };
			var adapter = new InMemoryAdapter(cache);
			var store = new DataStore(adapter);
			store.Map<Board>("board");

			var board = new
			{
				Id = 780,
				Title = "A new Board",
				Description = "Cool Description",
				GroupId = 1234,
				Colors = new List<string>() { "Red" }
			};

			var createdBoard = store.Create<Board>("board", board);
			var actualBoard = store.Find("board", 780);

			Assert.AreSame(actualBoard, createdBoard);

		}

		[Test]
		public void UpdateWorks()
		{

			var expectedBoards = CreateBoards(10).ToList();
			var cache = new Dictionary<string, IEnumerable<object>> { ["board"] = expectedBoards };
			var adapter = new InMemoryAdapter(cache);
			var store = new DataStore(adapter);
			store.Map<Board>("board");

			var board = new Board
			{
				Id = 9188,
				Title = "A new Board",
				GroupId = 1234,
				Colors = new List<string>() { "Red" }
			};

			var boardChanged = new Board()
			{
				Id = 9188,
				Title = "An Updated Board",
				Description = "Cool Description",
			};

			var createdBoard = store.Create("board", board);
			var actualBoard = store.Find("board", 9188);

			var updated = store.Update<Board>("board", boardChanged.Id, boardChanged);
			Assert.AreSame(actualBoard, createdBoard);
			Assert.AreSame(board, updated);
			Assert.AreNotSame(boardChanged, updated);

			Assert.AreEqual(9188, updated.Id);
			Assert.AreEqual("An Updated Board", updated.Title);
			Assert.AreEqual(0, updated.GroupId);
			Assert.AreEqual(null, updated.Colors);
			Assert.AreEqual("Cool Description", updated.Description);
		}

		[Test]
		public void UpdateGenericWorks()
		{

			var expectedBoards = CreateBoards(10).ToList();
			var cache = new Dictionary<string, IEnumerable<object>> { ["board"] = expectedBoards };
			var adapter = new InMemoryAdapter(cache);
			var store = new DataStore(adapter);
			store.Map<Board>("board");

			var board = new Board
			{
				Id = 9188,
				Title = "A new Board",
				GroupId = 1234,
				Colors = new List<string>() { "Red" }
			};

			var boardChanged = new
			{
				Id = 9188,
				Title = "An Updated Board",
				Description = "Cool Description",
			};

			var createdBoard = store.Create("board", board);
			var actualBoard = store.Find("board", 9188);

			var updated = (Board)store.Update("board", boardChanged.Id, boardChanged);
			Assert.AreSame(actualBoard, createdBoard);
			Assert.AreSame(board, updated);
			Assert.AreNotSame(boardChanged, updated);

			Assert.AreEqual(9188, updated.Id);
			Assert.AreEqual("An Updated Board", updated.Title);
			Assert.AreEqual(0, updated.GroupId);
			Assert.AreEqual(null, updated.Colors);
			Assert.AreEqual("Cool Description", updated.Description);
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
