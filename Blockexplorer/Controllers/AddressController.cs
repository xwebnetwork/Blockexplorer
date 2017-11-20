using System;
using System.Threading.Tasks;
using Blockexplorer.Core.Interfaces.Services;
using Blockexplorer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blockexplorer.Controllers
{
    public class AddressController : Controller
    {
	    readonly IAddressService _addressService;

	    const int ItemsOnPage = 20;

	    public AddressController(IAddressService addressService)
	    {
		    _addressService = addressService;
	    }

	    [Route("address/{id}")]
	    public async Task<ActionResult> Index(string id, string show, int page = 0)
	    {
			//if(show == null)
			//	return View("_NotFound");

			try
		    {
			    if (string.IsNullOrEmpty(id))
			    {
				    return RedirectToAction("Index", "Home");
			    }

			    var address = await _addressService.GetAddress(id);

			    if (address == null)
			    {
				    return View("_NotFound");
			    }

			    var start = ItemsOnPage * page;

			    long max;
			    if (start < address.TotalTransactions && start + ItemsOnPage < address.TotalTransactions)
			    {
				    max = start + ItemsOnPage;
			    }
			    else
			    {
				    max = address.TotalTransactions;
			    }

			    var vm = new AddressModel
			    {
				    Address = address,
				    Count = (int) Math.Ceiling((decimal) address.TotalTransactions / ItemsOnPage),
				    CurrentPage = page,
				    Start = start,
				    Max = max
			    };

			    return View(vm);
		    }
		    catch (Exception e)
		    {
				return View("_NotFound");
			}
		   
	    }
	}
}
