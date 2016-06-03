using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsData
{
	public interface IMapper<out T>
	{
		IQueryable<T> FindAll(IJsOptions opts = null);
		T Find(object id, IJsOptions opts = null);
		T Create(dynamic payload, IJsOptions opts = null);
		T Update(object id, dynamic payload, IJsOptions opts = null);
		Type Type { get; }
		string Name { get; set; }
		string IdAttribute { get; set; }
	}

	public class Mapper<T> : IMapper<T>
	{
		public string Name { get; set; }
		public string IdAttribute { get; set; } = "Id";
		private readonly MapperConfig<T> _mapperConfig;
		private readonly IAdapter _adapter;

		public Mapper(string name, IAdapter adapter, MapperConfig<T> mapperConfig = null)
		{
			Name = name;
			_adapter = adapter;
			_mapperConfig = mapperConfig;
		}

		public virtual bool BeforeFindAll(IBeforeFindOptions<T> opts)
		{
			return _mapperConfig?.BeforeFindAllExp?.Invoke(opts) ?? true;
		}

		public IQueryable<T> FindAll(IJsOptions opts = null)
		{
			var cont = true;

			cont = BeforeFindAll(new BeforeFindAllOptions<T>()
			{
				Query = opts?.Query
			});

			if (!cont) return null;

			return _adapter.FindAll(this, opts);
		}

		public T Find(object id, IJsOptions opts = null)
		{
			return _adapter.Find(this, id, opts);
		}

		public T Create(dynamic payload, IJsOptions opts = null)
		{
			if (!(payload is T))
			{
				payload = ((JObject)JObject.FromObject(payload)).ToObject<T>();
			}
			return _adapter.Create<T>(this, payload, opts);
		}

		public T Update(object id, dynamic payload, IJsOptions opts = null)
		{
			if (!(payload is T))
			{
				payload = ((JObject)JObject.FromObject(payload)).ToObject<T>();
			}
			return _adapter.Update(this, id, payload, opts);
		}

		public Type Type => typeof(T);
	}
}