using AccountData;
using EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(AccountContext accountContext, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) : ControllerBase
{
    [Route("Register")]
    [HttpPost]
    public async Task<ActionResult> RegisterAsync(string email, string password)
    {
        var user = new IdentityUser
        {
            UserName = email,
            Email = email
        };
        
        var result = await userManager.CreateAsync(user, password);
        
        if (result.Succeeded)
        {
            return Ok();
        }
        
        return BadRequest();
    }
    
    [Route("Login")]
    [HttpPost]
    public async Task<ActionResult> LoginAsync(string email, string password, bool rememberMe)
    {
        var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
        
        if (result.Succeeded)
        {
            return Ok();
        }
        
        return Unauthorized();
    }
}