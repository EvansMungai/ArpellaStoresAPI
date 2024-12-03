using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ArpellaStores.Services;

public interface IAuthenticationService
{
    Task<IResult> RegisterUser(UserManager<User> userManager, User model);
    Task<IResult> Login(SignInManager<User> signInManager, User model);
}
