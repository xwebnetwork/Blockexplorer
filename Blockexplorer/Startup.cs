using Blockexplorer.BlockProvider;
using Blockexplorer.BlockProvider.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blockexplorer.Services;
using Blockexplorer.BlockProvider.Rpc.Client;
using Blockexplorer.Core.Interfaces;
using Blockexplorer.Core.Interfaces.Services;
using Blockexplorer.Core.Repositories;
using Blockexplorer.Core.Stubs;

namespace Blockexplorer
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Adds services required for using options.
			services.AddOptions();
			// Register the IConfiguration instance which MyOptions binds against.
			services.Configure<RpcSettings>(Configuration.GetSection(nameof(RpcSettings)));

			//Registers the following lambda used to configure options.
			services.Configure<RpcSettings>(myOptions =>
			{
				myOptions.Url = "http://me:123@127.0.0.1:8332/"; // override values from appsettings json
			});

			// Add framework services.
			services.AddMvc();

			// Add custom services.
			services.AddTransient<IBlockService, BlockService>();
			services.AddTransient<IBlockchainDataProvider, ChainProvider>();
			services.AddTransient<ITransactionProvider, TransactionAdapter>();
			services.AddTransient<IBlockProvider, BlockAdapter>();
			services.AddTransient<IAddressProvider, AddressAdapter>();
			services.AddTransient<IAddressService, AddressService>();
			services.AddTransient<IAddressRepository, AddressRepository>();
			services.AddTransient<IBlockRepository, BlockRepositoryStub>();
			services.AddTransient<ITransactionService, TransactionService>();
			services.AddTransient<ITransactionRepository, TransactionRepositoryStub>();
			services.AddTransient<ISearchService, SearchService>();
			services.AddTransient<IApiService, ApiService>();
			services.AddTransient<IInfoAdapter, InfoAdapter>();
			services.AddTransient<IStakingInfoAdapter, StakingInfoAdapter>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseExceptionHandler("/Home/Error");

			//if (env.IsDevelopment())
			//{
			//	app.UseDeveloperExceptionPage();
			//	app.UseBrowserLink();
			//}
			//else
			//{
			//	app.UseExceptionHandler("/Home/Error");
			//}
		}
	}
}
