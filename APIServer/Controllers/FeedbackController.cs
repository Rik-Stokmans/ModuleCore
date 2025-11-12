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
    private static readonly List<KeyValuePair<string, DateTime>> FeedbackItems = [];
    public static readonly List<string> AdminScreensIds = ["C2D361003268"]; // Example admin screens
    
    
    [HttpPost]
    [Route("{screenId}/{condition}/{comment}")]
    public async Task<ActionResult> CreateLogAsync(string screenId, FeedbackConditions condition, string comment = "")
    {
        //check if the database contains the screenId
        if (!await context.ScreenLocations.AnyAsync(location => location.ScreenId == screenId))
        {
            return NotFound("screenId not found");
        }
        
        //check if there has been a request in the last 5 seconds
        var lastRequest = FeedbackItems.FirstOrDefault(item => item.Key == screenId);
        if (lastRequest.Key != null && lastRequest.Value > DateTime.Now.AddSeconds(-5))
        {
            return BadRequest("You can only send a request every 5 seconds");
        }
        
        await context.FeedbackConditions.AddAsync(new Feedback(condition, screenId, comment));
        await context.SaveChangesAsync();
        FeedbackItems.RemoveAll(item => item.Key == screenId);
        FeedbackItems.Add(new KeyValuePair<string, DateTime>(screenId, DateTime.Now));
    
        return Created();
    }
    
    
    [Authorize]
    [HttpGet]
    [Route("percentages/{startDate:datetime}/{endDate:datetime}")]
    public async Task<ActionResult<List<KeyValuePair<FeedbackConditions, double>>>> GetPercentages(DateTime startDate, DateTime endDate)
    {
        //if the screenId is "all" i want to collect all the condition submitted in the timeframe, otherwise i want only the conditions for the screenId
        var feedbackItemsList = await context.FeedbackConditions.Where(log => log.Time > startDate && log.Time < endDate).ToListAsync(); //only screens matching the screenId will be given
        
        //if the user does not have the admin role remove all admin screen ids from the list
        if (!User.IsInRole("Admin"))
        {
            feedbackItemsList.RemoveAll(item => AdminScreensIds.Contains(item.ScreenId));
        }
        
        var total = feedbackItemsList.Count;
        //for each condition, get the percentage of the total
        var percentages = feedbackItemsList.GroupBy(log => log.Condition)
            .Select(group => new KeyValuePair<FeedbackConditions, double>(group.Key, group.Count() / (double) total))
            .ToList();
        
        //order by percentage
        percentages = percentages.OrderByDescending(pair => pair.Value).ToList();
        
        return Ok(percentages);
    }
    
 
    [Authorize]
    [HttpGet]
    [Route("percentages/getAll")]
    //it should return a list of all the screens and their total percentage (0-100 of cleanlyness) and do this for the periods of today, this week, this month
    public async Task<ActionResult<List<KeyValuePair<string, List<double>>>>> GetAllPercentages()
    {
        //get all the screens
        var screens = await context.ScreenLocations.ToListAsync();
        
        //if the user does not have the admin role remove all admin screen ids from the list
        if (!User.IsInRole("Admin"))
        {
            screens.RemoveAll(item => AdminScreensIds.Contains(item.ScreenId));
        }
        
        //create a list of key value pairs with the screenId and a list of doubles with the percentages
        List<KeyValuePair<string, List<double>>> percentages = new List<KeyValuePair<string, List<double>>>();
        
        foreach (var screen in screens)
        {
            var screenId = screen.ScreenId;
            Console.WriteLine(screenId);
            
            //get the percentages for today, this week and this month
            var today = await context.FeedbackConditions.Where(log => log.ScreenId == screenId && log.Time > DateTime.Now.Date).ToListAsync();
            var thisWeek = await context.FeedbackConditions.Where(log => log.ScreenId == screenId && log.Time > DateTime.Now.Date.AddDays(-7)).ToListAsync();
            var thisMonth = await context.FeedbackConditions.Where(log => log.ScreenId == screenId && log.Time > DateTime.Now.Date.AddDays(-30)).ToListAsync();

            List<double> percentagesList = [GetTotalPercentageAverage(today), GetTotalPercentageAverage(thisWeek), GetTotalPercentageAverage(thisMonth)];
            
            //check if all the percentages are in the range of 0-1
            percentagesList.ForEach(percentage =>
            {
                //check if the percentage is in the range of 0-1 otherwise set it to 0
                if (percentage > 1)
                {
                    percentage = 1;
                }
                else if (percentage < 0)
                {
                    percentage = 0;
                }
            });
            
            //default these to 
            percentages.Add(new KeyValuePair<string, List<double>>(screenId, percentagesList));
        }
        
        return Ok(percentages);
    }
    
    
    [Authorize]
    [HttpGet]
    [Route("details/{screenId}")]
    public async Task<ActionResult<FeedbackDetailsView>> GetDetailsAsync(string screenId)
    {
        if (!await context.ScreenLocations.AnyAsync(location => location.ScreenId == screenId))
        {
            return NotFound("screenId not found");
        }

        var feedbackItemsList = await context.FeedbackConditions
            .Where(f => f.ScreenId == screenId)
            .OrderBy(f => f.Time)
            .ToListAsync();
        
        //if the user does not have the admin role remove all admin screen ids from the list
        if (!User.IsInRole("Admin"))
        {
            feedbackItemsList.RemoveAll(item => AdminScreensIds.Contains(item.ScreenId));
        }

        if (feedbackItemsList.Count == 0)
        {
            return Ok(new FeedbackDetailsView
            {
                ScreenId = screenId,
                LastActivity = DateTime.MinValue,
                Feedback = []
            });
        }

        var feedback = feedbackItemsList.Select(f => new FeedbackScorePoint
        {
            Timestamp = f.Time,
            Score = f.Condition switch
            {
                FeedbackConditions.VeryClean => 1.0,
                FeedbackConditions.Clean => 0.66,
                FeedbackConditions.Dirty => 0.33,
                FeedbackConditions.VeryDirty => 0.0,
                _ => 0.0
            },
            Comment = f.Comment
        }).ToList();

        var lastActivity = feedbackItemsList.Max(f => f.Time);

        return Ok(new FeedbackDetailsView
        {
            ScreenId = screenId,
            LastActivity = lastActivity,
            Feedback = feedback
        });
    }


    
    private double GetTotalPercentageAverage(List<Feedback> feedbackItems)
    {
        var total = feedbackItems.Count;
        
        if (total == 0)
        {
            return -1.0;
        }
        //for each condition, get the percentage of the total
        
        //summarize the percentages into a single value
        double totalValue = 0;
        
        foreach (var feedback in feedbackItems)
        {
            totalValue += feedback.Condition switch
            {
                FeedbackConditions.VeryClean => 1.0,
                FeedbackConditions.Clean => 0.66,
                FeedbackConditions.Dirty => 0.33,
                FeedbackConditions.VeryDirty => 0.0,
                _ => 0
            };
        }
        
        //calculate the average
        var average = totalValue / total;
        
        return average;
    }
}






















