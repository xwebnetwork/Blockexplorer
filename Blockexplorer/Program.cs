using System;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace Blockexplorer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
				.UseKestrel(options =>
				{
					options.ApplicationSchedulingMode = SchedulingMode.Inline;
					options.Listen(IPAddress.Any, 80);
					options.Listen(IPAddress.Any, 443,
				   listenOptions =>
						{
							try
							{
								listenOptions.NoDelay = true;
								var location = Assembly.GetEntryAssembly().Location;
								var directory = Path.GetDirectoryName(location);
								var passPath = Path.Combine(directory, "ssl.secret");
								if (File.Exists(passPath))
								{
									string pass = File.ReadAllText(passPath);
									var certPath = Path.Combine(directory, "wildcard_obsidianproject_org.pfx");
									if (File.Exists(certPath))
									{
										listenOptions.UseHttps("wildcard_obsidianproject_org.pfx", pass.Trim());
										listenOptions.UseConnectionLogging();
									}
									else
									{
										Console.WriteLine("File not found: " + certPath);
									}

								}


							}
							catch (Exception e) { Console.WriteLine(e.Message); }
						});
					options.UseSystemd();
				})

				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<Startup>()
				.Build();
			host.Run();
		}
		//.UseIISIntegration()
		//.UseUrls("http://*:80", "https://*:443")
		//.UseApplicationInsights()

		public static string Version => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
	}
}
