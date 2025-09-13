using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.SmsManagement.Services;

public class SmsHelpers : ISmsHelpers
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ArpellaContext _context;
    public SmsHelpers(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ArpellaContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }
    public async Task<List<string>> GetUsersInRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null) return new List<string>();
        var userIds = await _context.UserRoles.Where(ur => ur.RoleId == role.Id).Select(Ur => Ur.UserId).ToListAsync();

        var phoneNumbers = await _context.Users.Where(u => userIds.Contains(u.Id)).Select(u => u.UserName).ToListAsync();
        return phoneNumbers;
    }
}
