using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Examples.AspNetCore.Adapters;
using Examples.AspNetCore.Adapters.EntityFramework;
using Examples.Shared;
using JsData;
using JsData.Core.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Examples.AspNetCore.Controllers
{
	[Route("api/EF/{resource}")]
	public class EFController : Controller
	{

		public DataStore Store { get; set; }
		public EFController()
		{
			var adapter = new EntityFrameworkAdapter(new ExampleContext());
			Store = new DataStore(adapter);
			Store.Map<Board>();
			Store.Map<Post>();
			Store.Map<Pin>();
		}


		[HttpGet]
		protected IEnumerable<object> Get(string resource)
		{
			return Store.FindAll(resource);
		}
	}
}
