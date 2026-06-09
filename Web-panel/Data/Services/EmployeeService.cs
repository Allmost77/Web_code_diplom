using Diplom.Data.DbContext;
using Diplom.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Data.Services;

public sealed class EmployeeService(IDbContextFactory<AppDbContext> dbContextFactory)
{
    private readonly PasswordHasher<User> passwordHasher = new();

    public async Task<List<RoleOptionDto>> GetRolesAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .Select(role => new RoleOptionDto
            {
                Id = role.Id,
                Name = role.Name
            })
            .ToListAsync();
    }

    public async Task<CreateEmployeeResult> CreateEmployeeAsync(EmployeeCreateDto createDto)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var email = createDto.Email.Trim().ToLowerInvariant();
        var fullName = createDto.FullName.Trim();

        var roleExists = await dbContext.Roles.AnyAsync(role => role.Id == createDto.RoleId);
        if (!roleExists)
            return CreateEmployeeResult.RoleNotFound;

        var emailExists = await dbContext.Users
            .AnyAsync(user => user.Email != null && user.Email.ToLower() == email);

        if (emailExists)
            return CreateEmployeeResult.EmailAlreadyExists;

        var user = new User
        {
            FullName = fullName,
            Email = email,
            RoleId = createDto.RoleId
        };

        user.PasswordHash = passwordHasher.HashPassword(user, createDto.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return CreateEmployeeResult.Success;
    }
}

public enum CreateEmployeeResult
{
    Success,
    EmailAlreadyExists,
    RoleNotFound
}
