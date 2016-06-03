namespace JsData.Common
{
	public interface IJsOptions
	{
		JsQuery Query { get; set; }
	}

	public class JsOptions : IJsOptions
	{
		public JsQuery Query { get; set; }
	}
}