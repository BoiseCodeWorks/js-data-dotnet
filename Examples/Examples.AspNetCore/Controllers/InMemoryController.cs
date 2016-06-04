using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Examples.Shared;
using JsData;
using JsData.Core.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Examples.AspNetCore.Controllers
{
	[Route("api/InMemory/{resource}")]
	public class InMemoryController : JsDataController
	{
		private static readonly Dictionary<string, Dictionary<object, object>> Cache = CreateCache(10);

		public InMemoryController()
		{
			var adapter = new InMemoryAdapter(Cache);
			Store = new DataStore(adapter);
			Store.Map<Board>();
			Store.Map<Board>("board");
			Store.Map<Board>("board");
			Store.Map<Board>("board");
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
