using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Examples.Shared;
using JsData;
using Microsoft.AspNetCore.Mvc;

namespace Examples.AspNetCore.Controllers
{
	[Route("api/resource/{resource}")]
	public class ResourceController : Controller
	{
		private static readonly Dictionary<string, Dictionary<object, object>> Cache = CreateCache(10);

		private readonly DataStore _store;

		public ResourceController()
		{
			var adapter = new InMemoryAdapter(Cache);
			_store = new DataStore(adapter);
			_store.Map<Board>("board", config => { });
		}

		// GET api/values
		[HttpGet]
		public IEnumerable Get(string resource)
		{
			return _store.FindAll(resource);
		}

		[HttpGet("{id}")]
		public IActionResult Get(string resource, string id)
		{
			var results = _store.Find(resource, id);
			return new OkObjectResult(results);
		}

		[HttpPost]
		public IActionResult Post(string resource, [FromBody]dynamic props)
		{
			var result = _store.Create(resource, props);
			return new OkObjectResult(result);
		}

		[HttpPut("{id}")]
		public IActionResult Put(string resource, string id, [FromBody]dynamic props)
		{
			var result = _store.Update(resource, id, props);
			return new OkObjectResult(result);
		}

		[HttpDelete("{id}")]
		public void Delete(string resource, string id)
		{
		}


		public static IEnumerable<Board> CreateBoards(int number)
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

		private static Dictionary<string, Dictionary<object, object>> CreateCache(int i)
		{
			var cache = new Dictionary<string, Dictionary<object, object>>
			{
				["board"] = CreateBoards(10).ToDictionary<Board, object, object>(board => board.Id, board => board)
			};

			return cache;
		}
	}
}
