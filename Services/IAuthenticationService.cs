using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Services;

public interface IAuthenticationService
{
    Task<IResult> RegisterUser(UserManager<User> userManager, User model);
    Task<IResult> Login(string userName, string password);
}
