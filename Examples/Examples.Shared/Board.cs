using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Examples.Shared
{
	// This project can output the Class library as a NuGet Package.
	// To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
	public class Board : Record
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public virtual ICollection<Pin> Pins { get; set; }
	}

	public class Post : Record
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime CreatedDate { get; set; }
		public virtual ICollection<Pin> Pins { get; set; }
	}

	public class Pin : Record
	{
		public int Id { get; set; }
		public int BoardId { get; set; }
		public int PostId { get; set; }
		public DateTime CreatedDate { get; set; }
		public Board Board { get; set; }
		public Post Post { get; set; }
	}


	public class Record
	{

		[JsonExtensionData]
		[NotMapped]
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
