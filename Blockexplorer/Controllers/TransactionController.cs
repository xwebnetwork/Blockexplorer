using System;
using System.Threading.Tasks;
using Blockexplorer.Core.Interfaces.Services;
using Blockexplorer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blockexplorer.Controllers
{
    public class TransactionController : Controller
    {
	    readonly ITransactionService _transactionService;

	    public TransactionController(ITransactionService transactionService)
	    {
		    _transactionService = transactionService;
	    }

	    [Route("transaction/{id}")]
	    public async Task<ActionResult> Index(string id)
	    {
		    try
		    {
			    if (String.IsNullOrEmpty(id))
			    {
				    return RedirectToAction("Index", "Home");
			    }

			    var vm = await getTransactionVm(id);

			    if (vm.Transaction == null)
			    {
				    return View("_NotFound");
			    }

			    return View(vm);
		    }
		    catch (Exception e)
		    {
				return View("_NotFound");
			}
		  
	    }

	    public async Task<ActionResult> PartialTransactionDetails(string id)
	    {
		    var vm = await getTransactionVm(id);

		    if (vm.Transaction == null)
		    {
			    return View("_NotFound");
		    }

		    return View(vm);
	    }

	    private async Task<TransactionModel> getTransactionVm(string transactionId)
	    {
		    var transaction = await _transactionService.GetTransaction(transactionId);

		    var vm = new TransactionModel()
		    {
			    Transaction = transaction
		    };

		    return vm;
	    }
	}
}
