using GirafAPI.Data;
using GirafAPI.Entities.DTOs;
using GirafAPI.Entities.Users;
using GirafAPI.Entities.Users.DTOs;
using GirafAPI.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GirafAPI.Endpoints;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsersEndpoints(this WebApplication app)
    {
        //TODO Add authorization requirement to this group for users and admins
        var group = app.MapGroup("users");

        // POST /users
        group.MapPost("/", async (CreateUserDTO newUser, UserManager<GirafUser> userManager) =>
        {
            await Console.Out.WriteLineAsync(newUser.UserName);
            GirafUser user = newUser.ToEntity();
            var result = await userManager.CreateAsync(user, newUser.Password);

            return !result.Succeeded ? Results.BadRequest(result.Errors) :
                // LATER IMPLEMENTATION assign roles
                // await userManager.AddToRoleAsync(user, "RoleName");
                Results.Created($"/users/{user.Id}", user);
        });

        // PUT /users/{id}
        group.MapPut("/{id}",
            async (string id, UpdateUserDTO updatedUser, UserManager<GirafUser> userManager) =>
            {
                var user = await userManager.FindByIdAsync(id);
                
                if (user is null)
                {
                    return Results.NotFound();
                }
                
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;


                var result = await userManager.UpdateAsync(user);
                return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
            });

        //TODO Add auth so user can only change their own password unless they're an admin
        group.MapPut("/{id}/change-password", async (string id, UpdateUserPasswordDTO updatePasswordDTO, UserManager<GirafUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.ChangePasswordAsync(user, updatePasswordDTO.oldPassword, updatePasswordDTO.newPassword);

            return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
        });

        //TODO Add auth so a user can only change their own username unless they're an admin
        group.MapPut("/{id}/change-username",
            async (string id, UpdateUsernameDTO updateUsernameDTO, UserManager<GirafUser> userManager) =>
            {
                var user = await userManager.FindByIdAsync(id);
                var result = await userManager.SetUserNameAsync(user, updateUsernameDTO.Username);
                
                return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);

            });

        group.MapDelete("/{id}", async (string id, UserManager<GirafUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.DeleteAsync(user);
            return result.Succeeded ? Results.NoContent() : Results.BadRequest(result.Errors);
        });


        return group;
    }
}