using System.Threading.Tasks;
using Blockexplorer.Core.Enums;
using Blockexplorer.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blockexplorer.Controllers
{	
    public class SearchController : Controller
    {
	    readonly ISearchService _searchService;

	    public SearchController(ISearchService searchService)
	    {
		    _searchService = searchService;
	    }

	    [Route("search")]
	    public async Task<ActionResult> Index(string id)
	    {
		    var res = await _searchService.SearchEntityById(id);

		    switch (res)
		    {
			    case EntitySearchResult.Block:
				    return RedirectToAction("Index", "Block", new { id = id });
			    case EntitySearchResult.Transaction:
				    return RedirectToAction("Index", "Transaction", new { id = id });
			    case EntitySearchResult.Address:
				    return RedirectToAction("Index", "Address", new { id = id });
		    }

		    return View("_NotFound");
	    }
	}
}
