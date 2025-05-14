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
    private static List<KeyValuePair<string, DateTime>> feedbackItems = [];
    
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
        var lastRequest = feedbackItems.FirstOrDefault(item => item.Key == screenId);
        if (lastRequest.Key != null && lastRequest.Value > DateTime.Now.AddSeconds(-5))
        {
            return BadRequest("You can only send a request every 5 seconds");
        }
        
        await context.FeedbackConditions.AddAsync(new Feedback(condition, screenId, comment));
        await context.SaveChangesAsync();
        feedbackItems.RemoveAll(item => item.Key == screenId);
        feedbackItems.Add(new KeyValuePair<string, DateTime>(screenId, DateTime.Now));

        return Created();
    }
    
    //  !!!!TESTING ONLY!!!!!
    // [HttpPost]
    // [Route("insertTestData")]
    // public async Task<ActionResult> InsertTestData()
    // {
    //     var comments = new List<string>
    //     {
    //         "Technical-Problem", "Long-Line", "Unhygienic", "Amenities-Missing", "No-Comment"
    //     };
    //     //create feedback items with a time window of 1 hour and make enough items for 1 week back
    //     var feedbackItemsList = new List<Feedback>();
    //     var screenId = /*get all screens*/ await context.ScreenLocations.Select(location => location.ScreenId).ToListAsync();
    //     var random = new Random();
    //     var conditions = Enum.GetValues(typeof(FeedbackConditions)).Cast<FeedbackConditions>().ToList();
    //     var startTime = DateTime.Now.AddDays(-7);
    //     var endTime = DateTime.Now;
    //     var timeSpan = endTime - startTime;
    //     
    //     foreach (var se in screenId)
    //     {
    //         for (var i = 0; i < timeSpan.TotalHours; i++)
    //         {
    //             var condition = random.Next(0, 100) switch
    //             {
    //                 < 50 => conditions[0],
    //                 < 75 => conditions[1],
    //                 < 90 => conditions[2],
    //                 _ => conditions[3]
    //             };
    //             
    //             var comment = "No-Comment";
    //             if (condition != 0) {comment = comments[random.Next(0, comments.Count)];}
    //             var time = startTime.AddHours(i);
    //             feedbackItemsList.Add(new Feedback(condition, se, comment) { Time = time }); 
    //         }
    //     }
    //     
    //     
    //     //remove all database items
    //     var feedbackItemsListT = await context.FeedbackConditions.ToListAsync();
    //     context.FeedbackConditions.RemoveRange(feedbackItemsListT);
    //     
    //     //add the feedback items to the database
    //     await context.FeedbackConditions.AddRangeAsync(feedbackItemsList);
    //     await context.SaveChangesAsync();
    //     return Created();
    // }
    
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
    
    [Authorize]
    [HttpGet]
    [Route("comments/{screenId}/{startDate:datetime}/{endDate:datetime}")]
    public async Task<ActionResult<List<KeyValuePair<string, int>>>> GetComments(string screenId, DateTime startDate, DateTime endDate)
    {
        if (!await context.ScreenLocations.AnyAsync(location => location.ScreenId == screenId))
        {
            return NotFound("screenId not found");
        }
        
        // get all the comments for the screenId and filter them by the amount of that comment that exist
        List<KeyValuePair<string, int>> comments = context.FeedbackConditions
            .Where(log => log.ScreenId == screenId && log.Comment != "" && log.Time > startDate && log.Time < endDate)
            .AsEnumerable() // Forces client-side evaluation
            .GroupBy(log => log.Comment)
            .Select(group => new KeyValuePair<string, int>(group.Key, group.Count()))
            .OrderByDescending(pair => pair.Value)
            .ToList();

        
        return Ok(comments);
    }
    
    [Authorize]
    [HttpGet]
    [Route("comments/{startDate:datetime}/{endDate:datetime}")]
    public async Task<ActionResult<List<KeyValuePair<string, int>>>> GetComments(DateTime startDate, DateTime endDate)
    {
        // get all the comments for the screenId and filter them by the amount of that comment that exist
        List<KeyValuePair<string, int>> comments = context.FeedbackConditions
            .Where(log => log.Comment != "" && log.Time > startDate && log.Time < endDate)
            .AsEnumerable() // Forces client-side evaluation
            .GroupBy(log => log.Comment)
            .Select(group => new KeyValuePair<string, int>(group.Key, group.Count()))
            .OrderByDescending(pair => pair.Value)
            .ToList();
        
        return Ok(comments);
    }
    
    [Authorize]
    [HttpGet]
    [Route("percentages/getAll")]
    //it should return a list of all the screens and their total percentage (0-100 of cleanlyness) and do this for the periods of today, this week, this month
    public async Task<ActionResult<List<KeyValuePair<string, List<double>>>>> GetAllPercentages()
    {
        //get all the screens
        var screens = await context.ScreenLocations.ToListAsync();
        
        //create a list of keyvaluepairs with the screenId and a list of doubles with the percentages
        List<KeyValuePair<string, List<double>>> percentages = new List<KeyValuePair<string, List<double>>>();
        
        foreach (var screen in screens)
        {
            var screenId = screen.ScreenId;
            
            //get the percentages for today, this week and this month
            var today = await context.FeedbackConditions.Where(log => log.ScreenId == screenId && log.Time > DateTime.Now.Date).ToListAsync();
            var thisWeek = await context.FeedbackConditions.Where(log => log.ScreenId == screenId && log.Time > DateTime.Now.Date.AddDays(-7)).ToListAsync();
            var thisMonth = await context.FeedbackConditions.Where(log => log.ScreenId == screenId && log.Time > DateTime.Now.Date.AddDays(-30)).ToListAsync();

            List<double> percentagesList = [GetTotalPercentageAverage(today), GetTotalPercentageAverage(thisWeek), GetTotalPercentageAverage(thisMonth)];
            
            //check if all the percentages are in the range of 0-1
            percentagesList.ForEach(percentage =>
            {
                Console.WriteLine(percentage);
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






















