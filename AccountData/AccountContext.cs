using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountData;

public class AccountContext(DbContextOptions<AccountContext> options) : IdentityDbContext(options)
{
}