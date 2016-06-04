using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsData
{
	public class Record
	{

		[JsonExtensionData]
		public IDictionary<string, object> _Json { get; set; }

		[JsonIgnore]

		public string Json
		{
			get { return JsonConvert.SerializeObject(_Json); }
			set
			{
				if (string.IsNullOrEmpty(value)) return;

				var metaData = JsonConvert.DeserializeObject<IDictionary<string, object>>(value);

				_Json = metaData ?? new Dictionary<string, object>();
			}
		}
	}
}