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
    public async Task<ActionResult> CreateLogAsync(string message)
    {
        await context.LogMessages.AddAsync(new LogMessage(message));
        await context.SaveChangesAsync();

        return Created();
    }
    
    [HttpGet]
    public async Task<ActionResult<List<LoggingView>>> GetLogsAsync()
    {
        var logs = await context.LogMessages.ToListAsync();
        var logViews = logs.Select(log => log.GetLogView()).ToList();
        
        return Ok(logViews);
    }
}