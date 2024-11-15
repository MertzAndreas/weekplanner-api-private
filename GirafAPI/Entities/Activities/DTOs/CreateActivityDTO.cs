using System.ComponentModel.DataAnnotations;

namespace GirafAPI.Entities.Activities.DTOs;

public record CreateActivityDTO(
    [Required][StringLength(10)] string Date,
    [Required][StringLength(50)] string Name,
    [StringLength(500)] string Description,
    [Required][StringLength(10)] string StartTime,
    [Required][StringLength(10)] string EndTime
    );