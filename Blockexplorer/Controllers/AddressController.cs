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
		readonly IBlockService _blockService;

		const int ItemsOnPage = 20;

		public AddressController(IAddressService addressService, IBlockService blockService)
		{
			_addressService = addressService;
			_blockService = blockService;
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
					Count = (int)Math.Ceiling((decimal)address.TotalTransactions / ItemsOnPage),
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

		[Route("address/top")]
		[ResponseCache(Duration = 120, VaryByQueryKeys = new[] { "clear" })]
		public async Task<ActionResult> Top()
		{
			try
			{
				var top = await _addressService.GetTopList();
				var stats = await _addressService.GetStats();
				var lastBlock = await _blockService.GetLastBlock();

				int indexerHeight = stats.Item1;
				if (indexerHeight > lastBlock.Height) // best blockheight is cached and may thus lag indexer height
					indexerHeight = (int)lastBlock.Height;

				ViewData["BestHeight"] = indexerHeight;
				ViewData["IndexerLastSeenAgo"] = (DateTime.UtcNow - stats.Item2).TotalMinutes.ToString("0.0");
				ViewData["BestBlockHeight"] = lastBlock.Height;
				return View(top);
			}
			catch (Exception e)
			{
				return View("Error");
			}

		}
	}
}
