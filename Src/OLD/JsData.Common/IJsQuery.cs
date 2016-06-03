using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace JsData.Common
{
	public class JsQuery
	{
		public static Dictionary<string, Func<object, object, string>> Ops = new Dictionary<string, Func<object, object, string>>()
		{
			["=="] = (field, value) => $"{field} == {value}",
			["==="] = (field, value) => $"{field} == {value}",
			["!="] = (field, value) => $"{field} != {value}",
			["!=="] = (field, value) => $"{field} != {value}",
			[">"] = (field, value) => $"{field} > {value}",
			[">="] = (field, value) => $"{field} >= {value}",
			["<"] = (field, value) => $"{field} < {value}",
			["<="] = (field, value) => $"{field} <= {value}",
			["isectEmpty"] = (field, value) => $"{field} == {value}",
			["isectNotEmpty"] = (field, value) => $"{field} == {value}",
			["in"] = (field, value) => $"{value}.Contains({field})",
			["notIn"] = (field, value) => $"!{value}.Contains({field})",
			["contains"] = (field, value) => $"{field}.Contains({value})",
			["notContains"] = (field, value) => $"!{field}.Contains({value})"
		};

		public int? Limit { get; set; }
		public int? Offset { get; set; }
		public int? Skip { get; set; }
		public Dictionary<string, Dictionary<string, object>> Where { get; set; }
		public List<string[]> OrderBy { get; set; }
		//public Dictionary<string,> Where { get; set; }


		public static IQueryable<T> Filter<T>(IQueryable<T> data, JsQuery query)
		{
			var skip = query.Skip ?? query.Offset;

			if (query.Where != null && query.Where.Any())
			{
				var where = "";
				var values = new List<object>();
				foreach (var clause in query.Where)
				{
					var field = clause.Key;
					foreach (var opval in clause.Value)
					{
						var op = opval.Key;
						var andOr = " && ";
						if (op[0] == '|')
						{
							andOr = " || ";
							op = op.Substring(1);
						}
						andOr = string.IsNullOrWhiteSpace(where) ? "" : andOr;
						if (!Ops.ContainsKey(op)) continue;

						var exp = Ops[op](field, "@" + values.Count);
						values.Add(opval.Value);
						where += $"{andOr}{exp}";
					}
				}

				data = string.IsNullOrWhiteSpace(where) ? data : data.Where(where, values.ToArray());
			}

			if (skip.HasValue)
			{
				data = data.Skip(skip.Value);
			}

			if (query.Limit.HasValue)
			{
				data = data.Take(query.Limit.Value);
			}

			if (query.OrderBy != null && query.OrderBy.Any())
			{
				var orderBy = string.Join(",", query.OrderBy.Select(x => string.Join(" ", x)));
				data = data.OrderBy(orderBy);

			}

			return data;
		}
	}
}