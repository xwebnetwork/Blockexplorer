using System;
using System.Threading.Tasks;
using Blockexplorer.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Blockexplorer.Controllers
{
	public class ApiController : Controller
	{
		readonly IAddressService _addressService;
		readonly IApiService _apiService;
		const decimal Burn = 8800096.6m;
		const decimal Locked = 35266137m;


		public ApiController(IApiService apiService, IAddressService addressService)
		{
			_apiService = apiService;
			_addressService = addressService;
		}

		public async Task<ActionResult> GetAddress(string id)
		{
			try
			{
				var address = await _addressService.GetAddress(id);
				if (address == null)
					return StatusCode(StatusCodes.Status404NotFound);
				return Json(new { id = id, balance = address.Balance, totalTransactions = address.TotalTransactions, lastModifiedBlockHeight = address.LastModifiedBlockHeight });
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		public async Task<ActionResult> GetAddressWithTransactions(string id)
		{
			try
			{
				var address = await _addressService.GetAddress(id);
				if (address == null)
					return StatusCode(StatusCodes.Status404NotFound);
				return Json(new { id = id, balance = address.Balance, totalTransactions = address.TotalTransactions, lastModifiedBlockHeight = address.LastModifiedBlockHeight, txids = address.TxIds });
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		public async Task<ActionResult> Richlist()
		{
			try
			{
				var toplist = await _addressService.GetTopList();
				var compact = toplist.Select(x => new { Id = x.Id, Balance = x.Balance }).ToArray();
				return Json(compact);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		// [HttpGet]
		public async Task<ActionResult> GetInfo()
		{
			try
			{
				var info = await _apiService.GetInfo();
				if (info == null || !string.IsNullOrWhiteSpace(info.Errors))
					return StatusCode(StatusCodes.Status500InternalServerError);
				info.MoneySupply = info.MoneySupply - Burn;
				return Json(new { info = info });
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		// [HttpGet]
		public async Task<ActionResult> GetStakingInfo()
		{
			try
			{
				var info = await _apiService.GetStakingInfo();
				if (info == null || !string.IsNullOrWhiteSpace(info.Errors))
					return StatusCode(StatusCodes.Status500InternalServerError);
				return Json(new { info = info });
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		// [HttpGet]
		public async Task<ActionResult> MoneySupplyJson()
		{
			try
			{
				var info = await _apiService.GetInfo();
				if (info == null || !string.IsNullOrWhiteSpace(info.Errors))
					return StatusCode(StatusCodes.Status500InternalServerError);
				var withBurn = info.MoneySupply - Burn;

				return Json(new { moneySupply = Convert.ToInt32(withBurn) });
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		// [HttpGet]
		public async Task<string> MoneySupplyString()
		{
			try
			{
				var info = await _apiService.GetInfo();
				if (info == null || !string.IsNullOrWhiteSpace(info.Errors))
					return "-1";
				var withBurn = info.MoneySupply - Burn;
				return Convert.ToInt32(withBurn).ToString();
			}
			catch (Exception)
			{
				return "-1";
			}
		}

		// [HttpGet]
		public async Task<ActionResult> CirculatingSupplyJson()
		{
			try
			{
				var info = await _apiService.GetInfo();
				if (info == null || !string.IsNullOrWhiteSpace(info.Errors))
					return StatusCode(StatusCodes.Status500InternalServerError);
				//var circulatingSupply = info.MoneySupply - Burn - Locked;
				var circulatingSupply = 25000000m;

				return Json(new { circulatingSupply = Convert.ToInt32(circulatingSupply) });
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		// [HttpGet]
		public async Task<string> CirculatingSupplyString()
		{
			try
			{
				var info = await _apiService.GetInfo();
				if (info == null || !string.IsNullOrWhiteSpace(info.Errors))
					return "-1";
				//var circulatingSupply = info.MoneySupply - Burn - Locked;
				var circulatingSupply = 25000000m;
				return Convert.ToInt32(circulatingSupply).ToString();
			}
			catch (Exception)
			{
				return "-1";
			}
		}
	}
}
