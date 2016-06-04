using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace JsData
{

	public interface IAdapter
	{
		IQueryable<T> FindAll<T>(IMapper<T> mapper, IJsOptions opts = null) where T : class;
		T Find<T>(IMapper<T> mapper, object id, IJsOptions opts) where T : class;

		T Create<T>(IMapper<T> mapper, T props, IJsOptions opts) where T : class;
		T Update<T>(IMapper<T> mapper, object id, T props, IJsOptions opts) where T : class;
	}

	public class InMemoryAdapter : IAdapter
	{
		private readonly Dictionary<string, Dictionary<object, object>> _cache;
		public InMemoryAdapter(Dictionary<string, Dictionary<object, object>> cache = null)
		{
			_cache = cache ?? new Dictionary<string, Dictionary<object, object>>();
		}

		public IQueryable<T> FindAll<T>(IMapper<T> mapper, IJsOptions opts = null) where T : class
		{
			var queryable = GetResource(mapper.Name).Values.OfType<T>().AsQueryable();
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

			return (T)GetResource(mapper.Name)[actualId];
		}

		public T Create<T>(IMapper<T> mapper, T props, IJsOptions opts) where T : class
		{
			var id = props.GetType().GetRuntimeProperty(mapper.IdAttribute).GetValue(props);
			GetResource(mapper.Name)[id] = props;
			return props;
		}

		public T Update<T>(IMapper<T> mapper, object id, T props, IJsOptions opts) where T : class
		{
			var existing = Find(mapper, id, opts);
			existing.MergeWith(props);
			return existing;
		}

		private Dictionary<object, object> GetResource(string name)
		{
			return _cache[name] as Dictionary<object, object>;
		}
	}
}