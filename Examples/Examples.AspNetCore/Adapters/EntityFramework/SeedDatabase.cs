using System;
using System.Collections.Generic;
using System.Linq;
using Examples.AspNetCore.Adapters.EntityFramework;
using Examples.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class DataSeeder
{
	static readonly Random Ran = new Random(1);

	// TODO: Move this code when seed data is implemented in EF 7

	/// <summary>
	/// This is a workaround for missing seed data functionality in EF Core
	/// More info: https://github.com/aspnet/EntityFramework/issues/629
	/// </summary>
	/// <param name="app">
	/// An instance that provides the mechanisms to get instance of the database context.
	/// </param>
	public static void SeedData(this IApplicationBuilder app)
	{
		var db = app.ApplicationServices.GetService<ExampleContext>();
		if (db.Pins.Any()) return;
		var boards = CreateBoards(20);
		var posts = CreatePosts(100);
		var pins = CreatePins(0, 10, boards, posts);
		db.Pins.AddRange(pins);
		db.SaveChanges();
	}

	private static IEnumerable<Pin> CreatePins(int minimumPerBoard, int maximumPerBoard, IEnumerable<Board> boards, IEnumerable<Post> posts)
	{
		int id = 100;
		posts = posts.ToList();
		foreach (var board in boards)
		{
			var count = Ran.Next(minimumPerBoard, maximumPerBoard);
			foreach (var post in posts.OrderBy(x => Ran.Next(0, 2)).Take(count))
			{
				yield return new Pin
				{
					Id = ++id,
					BoardId = board.Id,
					Board = board,
					Post = post,
					PostId = post.Id,
					CreatedDate = DateTime.Now.AddHours(-id)
				};
			}
		}
	}

	private static IEnumerable<Post> CreatePosts(int number)
	{
		return Enumerable.Range(1, number).Select(i => new Post
		{
			Id = i,
			Content = "Post Content, would be nice if it were markdown",
			Title = "The #" + i + " Post Created",
			CreatedDate = DateTime.Now.AddDays(-Ran.Next(0, 5))
		});
	}

	private static IEnumerable<Board> CreateBoards(int number)
	{
		return Enumerable.Range(1, number).Select(i => new Board()
		{
			Id = i,
			Description = "This is the #" + i + " board.",
			Title = "Board #" + i,
		});
	}
}