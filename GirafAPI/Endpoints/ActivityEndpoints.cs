using GirafAPI.Data;
using GirafAPI.Entities.Weekplans;
using GirafAPI.Entities.Weekplans.DTOs;
using GirafAPI.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GirafAPI.Endpoints;

public static class ActivityEndpoints
{
    public static RouteGroupBuilder MapActivityEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("weekplan");
        
        // GET activities for one day for citizen
        group.MapGet("/{citizenId}", async (int citizenId, string date, GirafDbContext dbContext) =>
            await dbContext.Activities
                .Where(a => a.CitizenId == citizenId)
                .Where(a => a.Date == DateOnly.Parse(date))
                .OrderBy(a => a.StartTime)
                .Select(a => a.ToDTO())
                .AsNoTracking()
                .ToListAsync()
        )
        .WithName("GetActivitiesForCitizenOnDate")
        .WithDescription("Gets activities for a specific citizen on a given date.")
        .WithTags("Activities")
        .Produces<List<ActivityDTO>>(StatusCodes.Status200OK);
        
        // GET single activity
        group.MapGet("/activity/{id}", async (int id, GirafDbContext dbContext) => 
        {
            Activity? activity = await dbContext.Activities.FindAsync(id);
            
            return activity is null ? Results.NotFound() : Results.Ok(activity.ToDTO());
        })
        .WithName("GetActivityById")
        .WithDescription("Gets a specific activity by ID.")
        .WithTags("Activities")
        .Produces<ActivityDTO>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
        
        // POST new activity
        group.MapPost("/{citizenId}", async (int citizenId, CreateActivityDTO newActivityDto, GirafDbContext dbContext) => 
        {
            Activity activity = newActivityDto.ToEntity(citizenId);
                
            dbContext.Activities.Add(activity);
            await dbContext.SaveChangesAsync();
            return Results.Created($"/activity/{activity.Id}", activity.ToDTO());
        })
        .WithName("CreateActivity")
        .WithDescription("Creates a new activity for a citizen.")
        .WithTags("Activities")
        .Accepts<CreateActivityDTO>("application/json")
        .Produces<ActivityDTO>(StatusCodes.Status201Created);
        
        // PUT updated activity
        group.MapPut("/{id}", async (int id, UpdateActivityDTO updatedActivity, GirafDbContext dbContext) =>
        {
            var activity = await dbContext.Activities.FindAsync(id);

            if (activity is null)
            {
                return Results.NotFound();
            }
            
            dbContext.Entry(activity).CurrentValues.SetValues(updatedActivity.ToEntity(id));
            await dbContext.SaveChangesAsync();
            
            return Results.Ok();
        })
        .WithName("UpdateActivity")
        .WithDescription("Updates an existing activity using ID.")
        .WithTags("Activities")
        .Accepts<UpdateActivityDTO>("application/json")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
        
        // DELETE activity
        group.MapDelete("/activity/{id}", async (int id, GirafDbContext dbContext) =>
        {
            await dbContext.Activities.Where(a => a.Id == id).ExecuteDeleteAsync();
            
            return Results.NoContent();
        })
        .WithName("DeleteActivity")
        .WithDescription("Deletes an activity by ID.")
        .WithTags("Activities")
        .Produces(StatusCodes.Status204NoContent);

        return group;
    }
}