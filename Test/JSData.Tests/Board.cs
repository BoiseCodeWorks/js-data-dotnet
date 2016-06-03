using System.Collections.Generic;
using JsData;

namespace JSData.Tests
{
	public class Board : Record
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public List<string> Colors { get; set; }
		public int GroupId { get; set; }
	}
}