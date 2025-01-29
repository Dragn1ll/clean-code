using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public class PageController : ControllerBase
{
    [HttpGet("/")]
    public IActionResult ShowStartPage()
    {
        return File("frontend/index.html", "text/html");
    }

    [HttpGet("/dashboard")]
    public IActionResult ShowDashboard()
    {
        return File("frontend/dashboard.html", "text/html");
    }

    [HttpGet("/redactor/{documentId:guid}")]
    public IActionResult ShowRedactor()
    {
        return File("frontend/redactor.html", "text/html");
    }
}