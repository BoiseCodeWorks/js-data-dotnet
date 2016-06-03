//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace JSData.Core
//{
//	[Route("/api/data/{resource}")]
//	public class MapperController
//	{
//		private IDataStore _store;

//		public MapperController()
//		{
//			_store = new DataStore(new EntityFrameworkAdapter());
//			_store.Map<Board>("boards", x =>
//				x.BeforeFindAll(ctx =>
//				{
//					Console.WriteLine(ctx.Query);
//					return true;
//				})
//			);
//		}

//		public IEnumerable<object> Get(string resource, [FromQuery] IJsOptions opts = null)
//		{
//			Console.WriteLine(resource);
//			var results = _store.FindAll(resource, opts);
//			return results;
//		}

//		[HttpGet("{id}")]
//		public IActionResult Get(string resource, string id, [FromQuery] IJsOptions opts = null)
//		{
//			var result = _store.Find(resource, id, opts);

//			if (result == null)
//			{
//				return new NotFoundResult();
//			}

//			return new OkObjectResult(result);
//		}

//		[HttpPost]
//		public IActionResult Post(string resource, [FromBody] JObject payload, [FromQuery] IJsOptions opts = null)
//		{
//			var record = _store.Create(resource, payload, opts);
//			return new OkObjectResult(record);
//		}


//		[HttpPut("{id}")]
//		public IActionResult Post(string resource, string id, [FromBody] JObject payload, [FromQuery] IJsOptions opts = null)
//		{
//			var record = _store.Update(resource, payload, opts);
//			return new OkObjectResult(record);
//		}

//	}

//	public class DataStore : IDataStore
//	{
//		private readonly IDictionary<string, IMapper> _mappers = new Dictionary<string, IMapper>();
//		private IAdapter _adapter;

//		public DataStore(IAdapter adapter)
//		{
//			_adapter = adapter;
//		}

//		private IMapper<T> GetMapper<T>(string name)
//		{
//			return (IMapper<T>)_mappers.FirstOrDefault(x => x.Key == name).Value;
//		}

//		public IEnumerable<T> FindAll<T>(string resource, IJsOptions opts = null)
//		{
//			var cont = true;
//			var mapper = GetMapper<T>(resource);

//			cont = mapper.BeforeFindAll(new BeforeFindAllOptions<T>()
//			{
//				Query = opts?.Query
//			});

//			if (!cont) return null;

//			return _adapter.FindAll<T>(resource, opts);
//		}

//		public T Find<T>(string resource, string id, IJsOptions opts = null)
//		{
//			throw new NotImplementedException();
//		}

//		public T Create<T>(string resource, JObject payload, IJsOptions opts = null)
//		{
//			throw new NotImplementedException();
//		}

//		public T Update<T>(string resource, JObject payload, IJsOptions opts = null)
//		{
//			throw new NotImplementedException();
//		}

//		public IEnumerable<object> FindAll(string resource, IJsOptions opts = null)
//		{
//			return FindAll<object>(resource, opts);
//		}

//		public object Find(string resource, string id, IJsOptions opts = null)
//		{
//			throw new NotImplementedException();
//		}

//		public object Create(string resource, JObject payload, IJsOptions opts = null)
//		{
//			throw new NotImplementedException();
//		}

//		public object Update(string resource, JObject payload, IJsOptions opts = null)
//		{
//			throw new NotImplementedException();
//		}

//		public void Map<T>(string name, Action<IMapperConfig<T>> config)
//		{
//			var mapperConfig = new MapperConfig<T>();
//			config(mapperConfig);

//			_mappers[name] = new DynamicMapper<T>(mapperConfig);
//		}

//		public void Map<T>(string name, IMapper<T> mapper)
//		{
//			throw new NotImplementedException();
//		}
//	}

//	public interface IAdapter
//	{
//		IEnumerable<T> FindAll<T>(string resource, IJsOptions opts = null);
//	}

//	public class EntityFrameworkAdapter : IAdapter
//	{
//		private readonly Dictionary<string, IEnumerable<object>> _cache;

//		public EntityFrameworkAdapter(Dictionary<string, IEnumerable<object>> cache = null)
//		{
//			_cache = cache ?? new Dictionary<string, IEnumerable<object>>();
//		}

//		public IEnumerable<T> FindAll<T>(string resource, IJsOptions opts = null)
//		{
//			return (IEnumerable<T>)_cache[resource];
//		}
//	}

//	public class BeforeFindAllOptions<T> : IBeforeFindOptions<T>
//	{
//		public IJsQuery Query { get; set; }
//	}

//	public class DynamicMapper<T> : Mapper<T>
//	{
//		private readonly IMapperConfig<T> _mapperConfig;

//		public DynamicMapper(IMapperConfig<T> mapperConfig)
//		{
//			_mapperConfig = mapperConfig;
//		}

//		public override bool BeforeFindAll(IBeforeFindOptions<T> opts)
//		{
//			return _mapperConfig.BeforeFindAllExp?.Invoke(opts) ?? base.BeforeFindAll(opts);
//		}
//	}

//	public class Board : Record
//	{
//		public int Id { get; set; }
//		public string Title { get; set; }
//		public string Description { get; set; }
//	}

//	public interface IJsOptions
//	{
//		IJsQuery Query { get; set; }
//	}

//	public interface IDataStore
//	{
//		IEnumerable<T> FindAll<T>(string resource, IJsOptions opts = null);
//		T Find<T>(string resource, string id, IJsOptions opts = null);
//		T Create<T>(string resource, JObject payload, IJsOptions opts = null);
//		T Update<T>(string resource, JObject payload, IJsOptions opts = null);

//		IEnumerable<object> FindAll(string resource, IJsOptions opts = null);
//		object Find(string resource, string id, IJsOptions opts = null);
//		object Create(string resource, JObject payload, IJsOptions opts = null);
//		object Update(string resource, JObject payload, IJsOptions opts = null);
//		void Map<T>(string name, Action<IMapperConfig<T>> mapperConfig);
//		void Map<T>(string name, IMapper<T> mapper);

//	}

//	public interface IMapperConfig<T>
//	{
//		IMapperConfig<T> BeforeFindAll(Func<IBeforeFindOptions<T>, bool> func);
//		Func<IBeforeFindOptions<T>, bool> BeforeFindAllExp { get; set; }
//	}

//	public class MapperConfig<T> : IMapperConfig<T>
//	{
//		public Func<IBeforeFindOptions<T>, bool> BeforeFindAllExp { get; set; }

//		public IMapperConfig<T> BeforeFindAll(Func<IBeforeFindOptions<T>, bool> func)
//		{
//			BeforeFindAllExp = func;
//			return this;
//		}
//	}

//	public interface IBeforeFindOptions<T> : IJsOptions
//	{

//	}

//	public interface IJsQuery
//	{
//	}

//	public class Record
//	{

//		[JsonExtensionData]
//		public IDictionary<string, object> _Json { get; set; }

//		[JsonIgnore]
//		public string Json
//		{
//			get { return JsonConvert.SerializeObject(_Json); }
//			set
//			{
//				if (string.IsNullOrEmpty(value)) return;

//				var metaData = JsonConvert.DeserializeObject<IDictionary<string, object>>(value);

//				_Json = metaData ?? new Dictionary<string, object>();
//			}
//		}
//	}

//	public interface IMapper
//	{
//		bool BeforeFindAll<T>(IBeforeFindOptions<T> opts);
//	}

//	public interface IMapper<T> : IMapper
//	{
//		bool BeforeFindAll(IBeforeFindOptions<T> opts);
//	}

//	public class Mapper<T> : IMapper<T>
//	{
//		public virtual bool BeforeFindAll(IBeforeFindOptions<T> opts)
//		{
//			return BeforeFindAll<T>(opts);
//		}

//		public bool BeforeFindAll<T1>(IBeforeFindOptions<T1> opts)
//		{
//			return true;
//		}
//	}
//}
