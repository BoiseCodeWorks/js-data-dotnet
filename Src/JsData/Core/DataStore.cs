using System;
using System.Collections.Generic;
using System.Linq;

namespace JsData
{
	public interface IDataStore
	{
		IQueryable<T> FindAll<T>(string resource, IJsOptions opts = null);
		T Find<T>(string resource, object id, IJsOptions opts = null);
		T Create<T>(string resource, dynamic props, IJsOptions opts = null);
		T Update<T>(string resource, object id, dynamic props, IJsOptions opts = null);

		IQueryable<object> FindAll(string resource, IJsOptions opts = null);
		object Find(string resource, object id, IJsOptions opts = null);
		object Create(string resource, dynamic payload, IJsOptions opts = null);
		object Update(string resource, object id, dynamic payload, IJsOptions opts = null);

		void Map<T>(string name, Action<IMapperConfig<T>> mapperConfig);
		void Map<T>(string name, IMapper<T> mapper);
	}

	public class DataStore : IDataStore
	{
		public IAdapter Adapter { get; set; }
		private readonly IDictionary<string, IMapper<object>> _mappers = new Dictionary<string, IMapper<object>>();

		public DataStore(IAdapter adapter)
		{
			Adapter = adapter;
		}

		private IMapper<object> GetMapper(string name)
		{
			var map = _mappers.FirstOrDefault(x => x.Key == name).Value;
			return map;
		}

		public IQueryable<T> FindAll<T>(string resource, IJsOptions opts = null)
		{
			return GetMapper(resource).FindAll(opts) as IQueryable<T>;
		}

		public T Find<T>(string resource, object id, IJsOptions opts = null)
		{
			return (T)GetMapper(resource).Find(id, opts);
		}

		public T Create<T>(string resource, dynamic props, IJsOptions opts = null)
		{
			return (T)GetMapper(resource).Create(props, opts);
		}

		public T Update<T>(string resource, object id, dynamic props, IJsOptions opts = null)
		{
			return (T)GetMapper(resource).Update(id, props, opts);
		}

		public IQueryable<object> FindAll(string resource, IJsOptions opts = null)
		{
			return GetMapper(resource).FindAll(opts);
		}

		public object Find(string resource, object id, IJsOptions opts = null)
		{
			return GetMapper(resource).Find(id, opts);
		}

		public object Create(string resource, dynamic payload, IJsOptions opts = null)
		{
			return GetMapper(resource).Create(payload, opts);
		}

		public object Update(string resource, object id, dynamic payload, IJsOptions opts = null)
		{
			return GetMapper(resource).Update(id, payload, opts);
		}

		public void Map<T>(string name, Action<IMapperConfig<T>> config = null)
		{

			var mapperConfig = new MapperConfig<T>();
			config?.Invoke(mapperConfig);
			_mappers[name] = (IMapper<object>)new Mapper<T>(name, Adapter, mapperConfig);

		}

		public void Map<T>(string name, IMapper<T> mapper)
		{
			_mappers[name] = (IMapper<object>)mapper;
		}
	}
}
