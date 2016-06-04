using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Examples.AspNetCore.Adapters.EntityFramework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Examples.AspNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
				//.ConfigureServices(ConfigureServices)
				//.Configure(Configure)
				
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

	    private static void ConfigureServices(IServiceCollection services)
	    {
		    services.AddDbContext<ExampleContext>();
	    }

	    private static void Configure(IApplicationBuilder app)
	    {
			app.SeedData();
		}
	}
}
