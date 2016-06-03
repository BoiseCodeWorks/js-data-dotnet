using System;

namespace JsData
{
	public interface IMapperConfig<T>
	{
		IMapperConfig<T> BeforeFindAll(Func<IBeforeFindOptions<T>, bool> func);
		Func<IBeforeFindOptions<T>, bool> BeforeFindAllExp { get; set; }
	}

	public class MapperConfig<T> : IMapperConfig<T>
	{
		public Func<IBeforeFindOptions<T>, bool> BeforeFindAllExp { get; set; }

		public IMapperConfig<T> BeforeFindAll(Func<IBeforeFindOptions<T>, bool> func)
		{
			BeforeFindAllExp = func;
			return this;
		}
	}
}