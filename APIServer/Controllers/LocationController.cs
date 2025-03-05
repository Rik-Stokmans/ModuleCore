using EntityFramework;
using LogicLayer.Modules.LoggingModule.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.ModelViews;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationController(Context context) : ControllerBase
{
    //method to create a location
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateLocationAsync(string screenId)
    {
        await context.ScreenLocations.AddAsync(new Location(screenId));
        await context.SaveChangesAsync();

        return Created();
    }
    
    //method to get all locations
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<string>>> GetLocationsAsync()
    {
        var locations = await context.ScreenLocations.Select(location => location.ScreenId).ToListAsync();
        return Ok(locations);
    }
    
    //method to delete a location and all of its feedback
    // [Authorize]
    // [HttpDelete]
    // [Route("{screenId}")]
    // public async Task<ActionResult> DeleteLocationAsync(string screenId)
    // {
    //     var location = await context.ScreenLocations.FirstOrDefaultAsync(location => location.ScreenId == screenId);
    //     if (location == null)
    //     {
    //         return NotFound();
    //     }
    //     
    //     context.ScreenLocations.Remove(location);
    //     context.FeedbackConditions.RemoveRange(context.FeedbackConditions.Where(feedback => feedback.ScreenId == screenId));
    //     await context.SaveChangesAsync();
    //
    //     return Ok();
    // }
    
    
}