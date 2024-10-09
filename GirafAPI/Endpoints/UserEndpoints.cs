using GirafAPI.Entities.Users;
using GirafAPI.Entities.Users.DTOs;
using GirafAPI.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GirafAPI.Endpoints;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsersEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("users").RequireAuthorization("AdminPolicy");

        // POST /users
        group.MapPost("/", async (CreateUserDTO newUserDTO, UserManager<GirafUser> userManager) =>
        {
            var user = newUserDTO.ToEntity();
            var result = await userManager.CreateAsync(user, newUserDTO.Password);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            // LATER IMPLEMENTATION assign roles
            // await userManager.AddToRoleAsync(user, "RoleName");

            return Results.Created($"/users/{user.Id}", user);
        })
         .WithName("CreateUser")
        .WithTags("Users")
        .WithDescription("Creates a new user with the specified details. Requires administrative privileges.")
        .Accepts<CreateUserDTO>("application/json")
        .Produces<GirafUser>(StatusCodes.Status201Created)
        .Produces<IEnumerable<IdentityError>>(StatusCodes.Status400BadRequest);

        return group;
    }
}