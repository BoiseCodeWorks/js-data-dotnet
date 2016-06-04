#if (!NET45)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace JsData.Core.Mvc
{
	public abstract class JsDataController : Controller
	{
		protected IDataStore Store { get; set; }

		//protected JsDataController(IDataStore store)
		//{
		//	Store = store;
		//}

		protected JsDataController()
		{
		}

		[HttpGet]
		protected virtual IEnumerable Get(string resource, [FromQuery]IJsOptions opts = null)
		{
			return Store.FindAll(resource, opts);
		}

		[HttpGet("{id}")]
		protected virtual IActionResult Get(string resource, string id, [FromQuery]IJsOptions opts = null)
		{
			var results = Store.Find(resource, id, opts);
			return new OkObjectResult(results);
		}

		[HttpPost]
		protected virtual IActionResult Post(string resource, [FromBody]dynamic props, [FromQuery]IJsOptions opts = null)
		{
			var result = Store.Create(resource, props, opts);
			return new OkObjectResult(result);
		}

		[HttpPut("{id}")]
		protected virtual IActionResult Put(string resource, string id, [FromBody]dynamic props, [FromQuery]IJsOptions opts = null)
		{
			var result = Store.Update(resource, id, props, opts);
			return new OkObjectResult(result);
		}

		[HttpDelete("{id}")]
		protected virtual void Delete(string resource, string id, [FromQuery]IJsOptions opts = null)
		{
		}

	}
}
#endif