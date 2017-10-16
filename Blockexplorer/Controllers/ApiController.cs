using System;
using System.Threading.Tasks;
using Blockexplorer.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockexplorer.Controllers
{
  public class ApiController : Controller
  {
    readonly IApiService _apiService;

    public ApiController(IApiService apiService)
    {
      _apiService = apiService;
    }
    [HttpGet]
    public async Task<ActionResult> GetInfo()
    {
      try
      {
        var info = await _apiService.GetInfo();
        if (info == null || !string.IsNullOrWhiteSpace(info.Errors))
          return StatusCode(StatusCodes.Status500InternalServerError);
        return Json(new { info = info });
      }
      catch (Exception)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }
    [HttpGet]
    public async Task<ActionResult> MoneySupplyJson()
    {
      try
      {
        var info = await _apiService.GetInfo();
        if (info == null || !string.IsNullOrWhiteSpace(info.Errors))
          return StatusCode(StatusCodes.Status500InternalServerError);
        return Json(new { moneySupply = Convert.ToInt32(info.MoneySupply) });
      }
      catch (Exception)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }
    [HttpGet]
    public async Task<string> MoneySupplyString()
    {
      try
      {
        var info = await _apiService.GetInfo();
        if (info == null || !string.IsNullOrWhiteSpace(info.Errors))
          return "-1";
        return Convert.ToInt32(info.MoneySupply).ToString();
      }
      catch (Exception)
      {
        return "-1";
      }
    }
  }
}
