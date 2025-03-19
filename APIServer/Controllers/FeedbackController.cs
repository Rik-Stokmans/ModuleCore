using EntityFramework;
using LogicLayer.Modules.LoggingModule.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.ModelViews;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FeedbackController(Context context) : ControllerBase
{
    [HttpPost]
    [Route("{screenId}/{condition}")]
    public async Task<ActionResult> CreateLogAsync(string screenId, FeedbackConditions condition)
    {
        //check if the database contains the screenId
        if (!await context.ScreenLocations.AnyAsync(location => location.ScreenId == screenId))
        {
            return NotFound("screenId not found");
        }
        
        await context.FeedbackConditions.AddAsync(new Feedback(condition, screenId));
        await context.SaveChangesAsync();

        return Created();
    }
    
    [Authorize]
    [HttpGet]
    [Route("{amount:int:range(1,100)}")]
    public async Task<ActionResult<List<FeedbackView>>> GetLogsAsync(int amount = 20)
    {
        var feedbackItems = await context.FeedbackConditions.OrderByDescending(log => log.Time).Take(amount).ToListAsync();
        var feedbackItemViews = feedbackItems.Select(feedback => feedback.GetFeedbackView()).ToList();
        
        return Ok(feedbackItemViews);
    }
    
    [Authorize]
    [HttpGet]
    [Route("{screenId}/{amount:int:range(1,100)}")]
    public async Task<ActionResult<List<FeedbackView>>> GetLogsAsync(string screenId, int amount = 20)
    {
        //check if the database contains the screenId
        if (!await context.ScreenLocations.AnyAsync(location => location.ScreenId == screenId))
        {
            return NotFound("screenId not found");
        }
        
        var feedbackItems = await context.FeedbackConditions.Where(feedback => feedback.ScreenId == screenId).OrderByDescending(log => log.Time).Take(amount).ToListAsync();
        var feedbackItemViews = feedbackItems.Select(feedback => feedback.GetFeedbackView()).ToList();
        
        return Ok(feedbackItemViews);
    }
    
    [Authorize]
    [HttpGet]
    [Route("percentages/{screenId}/{daysBack:int:range(1,365)}")]
    public async Task<ActionResult<List<KeyValuePair<FeedbackConditions, double>>>> GetPercentages(string screenId, int daysBack = 7)
    {
        if (!await context.ScreenLocations.AnyAsync(location => location.ScreenId == screenId))
        {
            return NotFound("screenId not found");
        }
        
        //if the screenId is "all" i want to collect all the condition submitted in the timeframe, otherwise i want only the conditions for the screenId
        var feedbackItems = await context.FeedbackConditions.Where(log => log.Time > DateTime.Now.AddDays(-daysBack) && log.ScreenId == screenId).ToListAsync(); //only screens matching the screenId will be given
        
        var total = feedbackItems.Count;
        //for each condition, get the percentage of the total
        var percentages = feedbackItems.GroupBy(log => log.Condition)
            .Select(group => new KeyValuePair<FeedbackConditions, double>(group.Key, group.Count() / (double) total))
            .ToList();
        
        //order by percentage
        percentages = percentages.OrderByDescending(pair => pair.Value).ToList();
        
        return Ok(percentages);
    }
    
    [Authorize]
    [HttpGet]
    [Route("percentages/{daysBack:int:range(1,365)}")]
    public async Task<ActionResult<List<KeyValuePair<FeedbackConditions, double>>>> GetPercentages(int daysBack = 7)
    {
        //if the screenId is "all" i want to collect all the condition submitted in the timeframe, otherwise i want only the conditions for the screenId
        var feedbackItems = await context.FeedbackConditions.Where(log => log.Time > DateTime.Now.AddDays(-daysBack)).ToListAsync(); //only screens matching the screenId will be given
        
        var total = feedbackItems.Count;
        //for each condition, get the percentage of the total
        var percentages = feedbackItems.GroupBy(log => log.Condition)
            .Select(group => new KeyValuePair<FeedbackConditions, double>(group.Key, group.Count() / (double) total))
            .ToList();
        
        //order by percentage
        percentages = percentages.OrderByDescending(pair => pair.Value).ToList();
        
        return Ok(percentages);
    }
    
    //the same method as the percentages but then with a date range
    [Authorize]
    [HttpGet]
    [Route("percentages/{screenId}/{startDate:datetime}/{endDate:datetime}")]
    public async Task<ActionResult<List<KeyValuePair<FeedbackConditions, double>>>> GetPercentages(string screenId, DateTime startDate, DateTime endDate)
    {
        if (!await context.ScreenLocations.AnyAsync(location => location.ScreenId == screenId))
        {
            return NotFound("screenId not found");
        }
        
        //if the screenId is "all" i want to collect all the condition submitted in the timeframe, otherwise i want only the conditions for the screenId
        var feedbackItems = await context.FeedbackConditions.Where(log => log.Time > startDate && log.Time < endDate && log.ScreenId == screenId).ToListAsync(); //only screens matching the screenId will be given
        
        var total = feedbackItems.Count;
        //for each condition, get the percentage of the total
        var percentages = feedbackItems.GroupBy(log => log.Condition)
            .Select(group => new KeyValuePair<FeedbackConditions, double>(group.Key, group.Count() / (double) total))
            .ToList();
        
        //order by percentage
        percentages = percentages.OrderByDescending(pair => pair.Value).ToList();
        
        return Ok(percentages);
    }
    
    [Authorize]
    [HttpGet]
    [Route("percentages/{startDate:datetime}/{endDate:datetime}")]
    public async Task<ActionResult<List<KeyValuePair<FeedbackConditions, double>>>> GetPercentages(DateTime startDate, DateTime endDate)
    {
        //if the screenId is "all" i want to collect all the condition submitted in the timeframe, otherwise i want only the conditions for the screenId
        var feedbackItems = await context.FeedbackConditions.Where(log => log.Time > startDate && log.Time < endDate).ToListAsync(); //only screens matching the screenId will be given
        
        var total = feedbackItems.Count;
        //for each condition, get the percentage of the total
        var percentages = feedbackItems.GroupBy(log => log.Condition)
            .Select(group => new KeyValuePair<FeedbackConditions, double>(group.Key, group.Count() / (double) total))
            .ToList();
        
        //order by percentage
        percentages = percentages.OrderByDescending(pair => pair.Value).ToList();
        
        return Ok(percentages);
    }
    
}