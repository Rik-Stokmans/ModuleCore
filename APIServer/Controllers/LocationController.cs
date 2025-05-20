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
    [Route("{screenId}/{name}")]
    public async Task<ActionResult> CreateLocationAsync(string screenId)
    {
        await context.ScreenLocations.AddAsync(new Location(screenId));
        await context.SaveChangesAsync();

        return Created();
    }
    
    //USED
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<KeyValuePair<string, string>>>> GetLocationsAsync()
    {
        //return a list of key value pairs with the screenId and the name of the location
        var locations = await context.ScreenLocations
            .Select(location => new KeyValuePair<string, string>(location.ScreenId, location.Name))
            .ToListAsync();
        
        //if the user does not have the admin role remove all admin screen ids from the list
        if (!User.IsInRole("Admin"))
        {
            locations.RemoveAll(item => FeedbackController.AdminScreensIds.Contains(item.Key));
        }
        
        return Ok(locations);
    }
    
    //USED
    [Authorize]
    [HttpPost]
    [Route("ChangeName/{screenId}/{newName}")]
    public async Task<ActionResult> ChangeNameAsync(string screenId, string newName)
    {
        var location = await context.ScreenLocations.FirstOrDefaultAsync(location => location.ScreenId == screenId);
        if (location == null)
        {
            return NotFound();
        }
        
        location.Name = newName;
        await context.SaveChangesAsync();

        return Ok();
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