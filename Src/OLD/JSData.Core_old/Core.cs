//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace JSData.Core
//{
//	// This project can output the Class library as a NuGet Package.
//	// To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
//	public class Collection
//	{
//		public Collection()
//		{

//		}
//	}

//	public class Mapper
//	{
//		public Mapper(MapperConfig config)
//		{

//		}

//		public string IdAttribute { get; set; }
//		public string Name { get; set; }
//		public IDictionary<string, IAdapter> Adapters { get; set; }
//		public Container DataStore { get; set; }
//		public string DefaultAdapter { get; set; }
//		public IDictionary<string, Relation> Relations { get; set; }
//		public IEnumerable<Relation> RelationList { get; set; }

//		public IAdapter GetAdapter(string name)
//		{
//			if (!Adapters.ContainsKey(name))
//			{
//				throw new Exception("Missing adapter: " + name);
//			}

//			return Adapters[name];
//		}

//		public void RegisterAdapter(string name, IAdapter adapter, bool isDefault)
//		{
//			Adapters[name] = adapter;
//			if (isDefault)
//			{
//				DefaultAdapter = name;
//			}
//		}

//		public void OnAll(Action<object> action)
//		{
//			throw new NotImplementedException();
//		}

//		public bool Is(object relationData)
//		{
//			throw new NotImplementedException();
//		}

//		public void CreateRecord(object props)
//		{
//			foreach (var def in RelationList)
//			{
//				var relatedMapper = def.GetRelation();
//				var relationData = def.GetLocalField(props);
//				if (relationData != null && !relatedMapper.Is(relationData))
//				{
//					if (relationData is IEnumerable && !relatedMapper.Is(relationData))
//					{
//						Utils.Set(props, def.LocalField, relatedMapper.CreateRecord(relationData));
//					}

//				}
//			}


//		}
//	}

//	public interface IAdapter
//	{
//	}

//	public class MapperConfig
//	{

//	}

//	public class BelongsToRelation : Relation
//	{

//	}

//	public class Relation
//	{
//		public Mapper GetRelation()
//		{
//			throw new NotImplementedException();
//		}

//		public object GetLocalField(object props)
//		{
//			throw new NotImplementedException();
//		}

//		public object LocalField { get; set; }
//	}


//	public class Container
//	{
//		private readonly Dictionary<string, Mapper> _mappers = new Dictionary<string, Mapper>();
//		private readonly Dictionary<string, IAdapter> _adapters = new Dictionary<string, IAdapter>();

//		public virtual Mapper DefineMapper(string name, MapperConfig opts)
//		{
//			var mapper = new Mapper(opts);
//			_mappers[name] = mapper;
//			mapper.Name = name;
//			mapper.Adapters = _adapters;
//			mapper.DataStore = this;
//			mapper.OnAll((x) => { OnMapperEvent(name, x); });

//			// Setup the mapper's relations, including generating Mapper#relationList
//			// and Mapper#relationFields

//			return mapper;
//		}

//		private IDictionary<string, IAdapter> GetAdapters()
//		{
//			return _adapters;
//		}

//		private IAdapter GetAdapter(string name)
//		{

//			if (!_adapters.ContainsKey(name))
//			{
//				throw new Exception("Missing adapter: " + name);
//			}

//			return _adapters[name];
//		}

//		public Mapper GetMapper(string name)
//		{
//			if (!_mappers.ContainsKey(name))
//			{
//				throw new Exception("Missing mapper: " + name);
//			}

//			return _mappers[name];
//		}





//		public IEnumerable<Relation> RelationList { get; set; }

//		public void RegisterAdapter(string name, IAdapter adapter, bool isDefault)
//		{
//			_adapters[name] = adapter;
//			if (isDefault)
//			{
//				foreach (var mapper in _mappers.Values)
//				{
//					mapper.DefaultAdapter = name;
//				}
//			}
//		}


//		private void OnMapperEvent(string name, object o)
//		{
//			throw new NotImplementedException();
//		}
//	}

//	public static class Utils
//	{
//		public static void Set(object props, object localField, object createRecord)
//		{
//			throw new NotImplementedException();
//		}
//	}

//	public class DataStore : Container
//	{
//		private IDictionary<string, Collection> _collections;

//		public List<Dictionary<string, object>> Clear()
//		{
//			var removed = new List<Dictionary<string, object>>();
//			foreach (var collection in _collections)
//			{
//				removed = collection.RemoveAll();
//			}

//			return removed;
//		}

//		public override Mapper DefineMapper(string name, MapperConfig opts)
//		{
//			var mapper = base.DefineMapper(name, opts);
//			var collection = !_collections.ContainsKey(name) ? new LinkedCollection(this, mapper) : _collections[name];

//			//todo index stuff


//			var idAttribute = mapper.IdAttribute;
//			foreach (var relation in mapper.RelationList)
//			{

//			}
//		}
//	}

//	public class LinkedCollection : Collection
//	{
//		public LinkedCollection(DataStore dataStore, Mapper mapper)
//		{
//			throw new NotImplementedException();
//		}
//	}
//}
