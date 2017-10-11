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
					options.Listen(IPAddress.Any, 443,
				   listenOptions =>
						{
							try
							{
								listenOptions.UseHttps("wildcard_obsidianplatform_com.pfx", args[0]); // hk
							}
							catch (Exception) { }
						});
					options.Listen(IPAddress.Any,80);
				})
				.UseUrls("http://*:80")
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				//.UseApplicationInsights()

				.Build();

			host.Run();
		}

		public static string Version => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
	}
}
