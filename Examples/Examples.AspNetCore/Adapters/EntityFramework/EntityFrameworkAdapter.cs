using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using JsData;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Examples.AspNetCore.Adapters
{
	public class EntityFrameworkAdapter : IAdapter
	{
		private readonly DbContext _context;

		public EntityFrameworkAdapter(DbContext context)
		{
			_context = context;
		}

		public IQueryable<T> FindAll<T>(IMapper<T> mapper, IJsOptions opts = null) where T : class
		{
			var queryable = _context.Set<T>().AsQueryable();
			if (opts?.Query != null)
			{
				queryable = JsQuery.Filter(queryable, opts.Query);
			}

			return queryable;
		}

		public T Find<T>(IMapper<T> mapper, object id, IJsOptions opts) where T : class
		{
			// convert the ID to the correct type
			var idType = typeof(T).GetRuntimeProperties().First(x => x.Name.Equals(mapper.IdAttribute, StringComparison.CurrentCultureIgnoreCase)).PropertyType;
			var actualId = Convert.ChangeType(id, idType);

			return FindAll(mapper, opts).Where($"{mapper.IdAttribute} == {actualId}").FirstOrDefault();
		}

		public T Create<T>(IMapper<T> mapper, T props, IJsOptions opts) where T : class
		{
			//var id = props.GetType().GetRuntimeProperty(mapper.IdAttribute).GetValue(props);
			_context.Set<T>().Add(props);
			return props;
		}

		public T Update<T>(IMapper<T> mapper, object id, T props, IJsOptions opts) where T : class
		{
			var existing = Find(mapper, id, opts);
			existing.MergeWith(props);
			return props;
		}
	}
}
