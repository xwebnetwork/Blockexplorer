using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;

namespace Blockexplorer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
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
