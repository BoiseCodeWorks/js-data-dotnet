using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsData.Common;
using System.Linq.Dynamic.Core;

namespace JsData.Core
{

	public interface IAdapter
	{
		IQueryable<T> FindAll<T>(IMapper<T> mapper, IJsOptions opts = null);
		T Find<T>(IMapper<T> mapper, object id, IJsOptions opts);

		T Create<T>(IMapper<T> mapper, T props, IJsOptions opts);
		T Update<T>(IMapper<T> mapper, object id, T props, IJsOptions opts);
	}

	public class InMemoryAdapter : IAdapter
	{
		private readonly Dictionary<string, IEnumerable<object>> _cache;

		public InMemoryAdapter(Dictionary<string, IEnumerable<object>> cache = null)
		{
			_cache = cache ?? new Dictionary<string, IEnumerable<object>>();
		}

		public IQueryable<T> FindAll<T>(IMapper<T> mapper, IJsOptions opts = null)
		{
			var queryable = GetResource<T>(mapper.Name).AsQueryable();
			if (opts?.Query != null)
			{
				queryable = JsQuery.Filter(queryable, opts.Query);
			}

			return queryable;
		}

		public T Find<T>(IMapper<T> mapper, object id, IJsOptions opts)
		{
			// convert the ID to the correct type
			var idType = typeof(T).GetProperty(mapper.IdAttribute).PropertyType;
			var actualId = Convert.ChangeType(id, idType);

			return FindAll(mapper, opts).Where($"Id == {actualId}").FirstOrDefault();
		}

		public T Create<T>(IMapper<T> mapper, T props, IJsOptions opts)
		{
			var newList = GetResource<T>(mapper.Name).ToList();
			newList.Add(props);
			_cache[mapper.Name] = newList as IEnumerable<object>;
			return props;
		}

		public T Update<T>(IMapper<T> mapper, object id, T props, IJsOptions opts)
		{
			var existing = Find(mapper, id, opts);
			MergeWith(existing, props);
			return existing;
		}

		private IEnumerable<T> GetResource<T>(string name)
		{
			return _cache[name] as IEnumerable<T>;
		}

		public void MergeWith<T>(T primary, T secondary)
		{
			//Utils.ObjectDiffPatch.PatchObject(primary, JObject.FromObject(secondary));
			foreach (var pi in typeof(T).GetProperties())
			{
				var priValue = pi.GetGetMethod().Invoke(primary, null);
				var secValue = pi.GetGetMethod().Invoke(secondary, null);
				if (priValue == null || priValue != secValue || pi.PropertyType.IsInstanceOfType(Activator.CreateInstance(pi.PropertyType)))
				{
					pi.GetSetMethod().Invoke(primary, new object[] { secValue });
				}
			}

			//var typeB = typeof (T);
			//foreach (PropertyInfo property in secondary.GetType().GetProperties())
			//{
			//	if (!property.CanRead || (property.GetIndexParameters().Length > 0))
			//		continue;

			//	PropertyInfo other = typeB.GetProperty(property.Name);
			//	if ((other != null) && (other.CanWrite))
			//		other.SetValue(primary, property.GetValue(secondary, null), null);
			//}
		}
	}
}