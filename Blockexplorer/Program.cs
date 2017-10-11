using System;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;

namespace Blockexplorer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
				.UseKestrel(options =>
				{
					options.Listen(IPAddress.Any, 80);
					options.Listen(IPAddress.Any, 443,
				   listenOptions =>
						{
							try
							{
								listenOptions.NoDelay = true;
								listenOptions.UseHttps("wildcard_obsidianplatform_com.pfx", args[0]); // hk
							}
							catch (Exception) { }
						});

				})
				//.UseUrls("http://*:80", "https://*:443")
				.UseContentRoot(Directory.GetCurrentDirectory())
				//.UseIISIntegration()
				.UseStartup<Startup>()
				//.UseApplicationInsights()

				.Build();

			host.Run();
		}

		public static string Version => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
	}
}
