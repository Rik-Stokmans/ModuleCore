using LogicLayer.Modules.LoggingModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleBuilder.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModuleBuilderController : Controller
{
    [Authorize]
    [HttpGet]
    public Task<ActionResult<List<ModuleHtmlObject>>> GetModuleHtml()
    {
        List<ModuleHtmlObject> modules = new();
        
        modules.Add(LoggingLayout.GetModuleHtml());

        return Task.FromResult<ActionResult<List<ModuleHtmlObject>>>(Ok(modules));
    }
}