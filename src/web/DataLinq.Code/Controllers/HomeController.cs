using E.DataLinq.Code.Services;
using E.DataLinq.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace E.DataLinq.Controllers;

public class HomeController : Controller
{
    private readonly DataLinqCodeService? _dataLinqCode;

    public HomeController(DataLinqCodeService? dataLinqCode = null)
    {
        _dataLinqCode = dataLinqCode;
    }

    public IActionResult Index()
    {
        var instances = _dataLinqCode?.Instances;
        var first = instances?.FirstOrDefault();

        if (first != null)
        {
            return Redirect(first.LoginUrl);
        }

        return View(instances);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}