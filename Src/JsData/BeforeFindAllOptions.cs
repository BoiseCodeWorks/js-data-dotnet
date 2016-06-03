namespace JsData
{
	public interface IBeforeFindOptions<T> : IJsOptions
	{

	}

	public class BeforeFindAllOptions<T> : IBeforeFindOptions<T>
	{
		public JsQuery Query { get; set; }
	}
}