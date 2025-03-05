using EntityFramework;
using LogicLayer.Modules.LoggingModule.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.ModelViews;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogsController(Context context) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [Route("{message}")]

    public async Task<ActionResult> CreateLogAsync(string message)
    {
        await context.LogMessages.AddAsync(new LogMessage(message));
        await context.SaveChangesAsync();

        return Created();
    }
    
    [HttpGet]
    [Route("{amount:int:range(1,100)}")]
    public async Task<ActionResult<List<LoggingView>>> GetLogsAsync(int amount = 20)
    {
        var logs = await context.LogMessages.OrderByDescending(log => log.Time).Take(amount).ToListAsync();
        var logViews = logs.Select(log => log.GetLogView()).ToList();
        
        return Ok(logViews);
    }
}