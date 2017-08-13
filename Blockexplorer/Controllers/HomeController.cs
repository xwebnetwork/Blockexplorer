using System.Threading.Tasks;
using Blockexplorer.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Blockexplorer.Models;

namespace Blockexplorer.Controllers
{
	public class HomeController : Controller
    {
	    public IBlockService BlockService { get; set; }

	    public HomeController(IBlockService blockService)
	    {
		    BlockService = blockService;
	    }

		public async Task<IActionResult> Index()
        {
	        var lastBlock = await BlockService.GetLastBlock();

	        var vm = new IndexModel
	        {
		        LastBlock = lastBlock,
	        };
			return View(vm);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
