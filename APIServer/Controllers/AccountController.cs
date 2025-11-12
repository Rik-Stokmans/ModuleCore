using AccountData;
using EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(AccountContext accountContext, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) : ControllerBase
{
    // ONLY FOR ACCOUNT CREATION
    [Route("Register")]
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RegisterAsync(string email, string password)
    {
        var user = new IdentityUser
        {
            UserName = email,
            Email = email
        };
        
        
        var result = await userManager.CreateAsync(user, password);
        
        //add the user to the user role
        await userManager.AddToRoleAsync(user, "User");
        
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
    
    [Route("Logout")]
    [HttpPost]
    public async Task<ActionResult> LogoutAsync()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }
    
    [Route("AddToRole")]
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AddToRoleAsync(string email, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            return NotFound();
        }
        
        var result = await userManager.AddToRoleAsync(user, role);
        
        if (result.Succeeded)
        {
            return Ok();
        }
        
        return BadRequest();
    }
    
    [Route("RemoveUser")]
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RemoveUserAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            return NotFound();
        }
        
        var result = await userManager.DeleteAsync(user);
        
        if (result.Succeeded)
        {
            return Ok();
        }
        
        return BadRequest();
    }
}