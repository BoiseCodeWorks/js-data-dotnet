using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JsData;
using Newtonsoft.Json;
using NUnit.Framework;
namespace JSData.Tests
{
	[TestFixture]
	public class QueryTests
	{
		private IQueryable<Board> _boards;

		public QueryTests()
		{
			_boards = CreateBoards(20).AsQueryable();
		}

		[Test]
		public void WhereTest()
		{
			var boards = _boards.ToList();
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
			var expected = boards.Where(x => x.Id == 12 || x.Id >= 17);
			var results = JsQuery.Filter(boards.AsQueryable(), query).ToList();

			Assert.AreEqual(expected, results);
		}

		[TestCase(1, "Id", 1)]
		[TestCase("1", "id", 1)]
		[TestCase("\"Board #1\"", "Title", 1)]
		[TestCase("\"Board #001\"", "Title", 0)]
		[TestCase(900, "id", 0)]
		public void OpEqualsTest(object value, string field, int expectedCount)
		{
			var op = "==";

			var actual = _boards.Where(JsQuery.Ops[op](field, value)).ToList();
			Assert.AreEqual(expectedCount, actual.Count);
		}

		[TestCase("==", 1, "Id", 1)]
		[TestCase("===", 1, "id", 1)]
		[TestCase("!=", 1, "id", 19)]
		[TestCase("!==", 1, "id", 19)]
		[TestCase(">", 10, "id", 10)]
		[TestCase(">=", 10, "id", 11)]
		[TestCase("<", 10, "id", 9)]
		[TestCase("<=", 10, "id", 10)]
		[TestCase(">=", 100, "id", 0)]
		public void OpCompareOpsTest(string op, object value, string field, int expectedCount)
		{
			var actual = _boards.Where(JsQuery.Ops[op](field, value)).ToList();
			Assert.AreEqual(expectedCount, actual.Count);
		}

		[TestCase("\"Red\"", "Colors", 20)]
		[TestCase("\"#1\"", "Description", 11)]
		[TestCase("\"0\"", "Description", 2)]
		[TestCase("\"007\"", "Title", 0)]

		public void OpContainsTest(object value, string field, int expectedCount)
		{
			var op = "contains";
			var actual = _boards.Where(JsQuery.Ops[op](field, value)).ToList();
			Assert.AreEqual(expectedCount, actual.Count);
		}

		[TestCase("\"Red\"", "Colors", 0)]
		[TestCase("\"#1\"", "Description", 9)]
		[TestCase("\"0\"", "Description", 18)]
		[TestCase("\"007\"", "Title", 20)]
		public void OpNotContainsTest(object value, string field, int expectedCount)
		{
			var op = "notContains";
			var actual = _boards.Where(JsQuery.Ops[op](field, value)).ToList();
			Assert.AreEqual(expectedCount, actual.Count);
		}

		//[Test]
		//public void OpInTest()
		//{
		//	var op = "in";
		//	var exp = JsQuery.Ops[op]("Id", "@0");
		//	var ids = new[] { 3, 4, 5 }.ToList();
		//	var actual = _boards.Where(exp, ids).ToList();
		//	Assert.AreEqual(3, actual.Count);
		//}

		//[Test]
		//public void OpNotInTest()
		//{
		//	var op = "notIn";
		//	var exp = JsQuery.Ops[op]("Id", "@0");
		//	var ids = new[] { 3, 4, 5 }.ToList();
		//	var actual = _boards.Where(exp, ids).ToList();
		//	Assert.AreEqual(17, actual.Count);
		//}


		[Test]
		public void OrderByTests()
		{
			var json = @"{""orderBy"": [[""GroupId""],[""Id"",""DESC""]]}";
			var query = JsonConvert.DeserializeObject<JsQuery>(json);

			var boards = _boards.ToList();
			var expected = boards.OrderBy(x => x.GroupId).ThenByDescending(x => x.Id).ToList();
			var results = JsQuery.Filter(boards.AsQueryable(), query).ToList();
			Assert.AreEqual(expected, results);

			expected = boards.OrderByDescending(x => x.Id).ToList();
			results = results.AsQueryable().OrderBy("Id DESC").ToList();
			Assert.AreEqual(expected, results);

			expected = results.OrderBy(x => x.GroupId).ThenBy(x => x.Id).ToList();
			results = results.AsQueryable().OrderBy("GroupId, Id").ToList();

			Assert.AreEqual(expected, results);

			expected = results.OrderByDescending(x => x.GroupId).ThenByDescending(x => x.Description).ToList();
			results = results.AsQueryable().OrderBy("GroupId DESC, Description DESC").ToList();

			Assert.AreEqual(expected, results);
		}

		[TestCase(5, 5, null)]
		[TestCase(5, null, 5)]
		[TestCase(2, 10, null)]
		[TestCase(2, null, null)]
		[TestCase(null, null, null)]
		[TestCase(null, 5, 2)]
		public void PagingTests(int? limit, int? offset, int? skip)
		{
			var boards = _boards.ToList();
			var json = "{\"limit\":\"" + limit + "\", \"offset\":\"" + offset + "\", \"skip\":\"" + skip + "\"}";
			var query = JsonConvert.DeserializeObject<JsQuery>(json);

			var expected = boards.Skip(skip ?? offset ?? 0);
			if (limit.HasValue) expected = expected.Take(limit.Value).ToList();
			var results = JsQuery.Filter(boards.AsQueryable(), query).ToList();
			Assert.AreEqual(expected, results);
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
					Colors = new[] { "Red", "Green", "Yellow" }.ToList(),
					GroupId = i % 3
				};
			}
		}
	}
}
