using System;
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
	        try
	        {
		        var lastBlock = await BlockService.GetLastBlock();

		        var vm = new IndexModel
		        {
			        LastBlock = lastBlock,
		        };
		        return View(vm);
	        }
	        catch (Exception e)
	        {
				return View("_NotFound");
			}
	      
        }

      

        public IActionResult Error()
        {
            return View();
        }
    }
}
