using System;
using System.Collections.Generic;
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

		static TopListData TopListCache;

		[Route("address/top")]
		[ResponseCache(Duration = 120)]
		public async Task<ActionResult> Top()
		{
			try
			{
				TopListData data;
				if (TopListCache != null && DateTime.UtcNow - TopListCache.DateTime <= TimeSpan.FromSeconds(120))
					data = TopListCache;
				else
				{
					data = new TopListData()
					{
						DateTime = DateTime.UtcNow,
						TopList = await _addressService.GetTopList(),
						Stats = await _addressService.GetStats(),
						LastBlock = await _blockService.GetLastBlock()
					};
					TopListCache = data;
				}

				int indexerHeight = data.Stats.Item1;
				if (indexerHeight > data.LastBlock.Height) // best blockheight is cached and may thus lag indexer height
					indexerHeight = (int)data.LastBlock.Height;

				ViewData["BestHeight"] = indexerHeight;
				ViewData["IndexerLastSeenAgo"] = (DateTime.UtcNow - data.Stats.Item2).TotalMinutes.ToString("0.0");
				ViewData["BestBlockHeight"] = data.LastBlock.Height;
				return View(data.TopList);
			}
			catch (Exception e)
			{
				return View("Error");
			}

		}

		class TopListData
		{
			public DateTime DateTime;

			public List<Core.Domain.Address> TopList;
			public Tuple<int, DateTime> Stats;
			public Core.Domain.Block LastBlock;
		}
	}
}
