using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace JsData
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
		private readonly Dictionary<string, Dictionary<object, object>> _cache;
		public InMemoryAdapter(Dictionary<string, Dictionary<object, object>> cache = null)
		{
			_cache = cache ?? new Dictionary<string, Dictionary<object, object>>();
		}

		public IQueryable<T> FindAll<T>(IMapper<T> mapper, IJsOptions opts = null)
		{
			var queryable = GetResource(mapper.Name).Values.OfType<T>().AsQueryable();
			if (opts?.Query != null)
			{
				queryable = JsQuery.Filter(queryable, opts.Query);
			}

			return queryable;
		}

		public T Find<T>(IMapper<T> mapper, object id, IJsOptions opts)
		{
			// convert the ID to the correct type
			var idType = typeof(T).GetRuntimeProperties().First(x => x.Name.Equals(mapper.IdAttribute, StringComparison.CurrentCultureIgnoreCase)).PropertyType;
			var actualId = Convert.ChangeType(id, idType);

			return (T)GetResource(mapper.Name)[actualId];
		}

		public T Create<T>(IMapper<T> mapper, T props, IJsOptions opts)
		{
			var id = props.GetType().GetRuntimeProperty(mapper.IdAttribute).GetValue(props);
			GetResource(mapper.Name)[id] = props;
			return props;
		}

		public T Update<T>(IMapper<T> mapper, object id, T props, IJsOptions opts)
		{
			var existing = Find(mapper, id, opts);
			MergeWith(existing, props);
			return existing;
		}

		private Dictionary<object, object> GetResource(string name)
		{
			return _cache[name] as Dictionary<object, object>;
		}

		public void MergeWith<T>(T primary, T secondary)
		{
			//Utils.ObjectDiffPatch.PatchObject(primary, JObject.FromObject(secondary));
			foreach (var pi in typeof(T).GetProperties())
			{
				var priValue = pi.GetGetMethod().Invoke(primary, null);
				var secValue = pi.GetGetMethod().Invoke(secondary, null);
				if (priValue == null || priValue != secValue || (pi.PropertyType.IsInstanceOfType(Activator.CreateInstance(pi.PropertyType))))
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