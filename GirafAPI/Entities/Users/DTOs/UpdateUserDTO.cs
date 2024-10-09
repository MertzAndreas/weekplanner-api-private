using System.ComponentModel.DataAnnotations;

namespace GirafAPI.Entities.DTOs;

public record UpdateUserDTO
(
    [Required][StringLength(100)] string FirstName,
    [Required][StringLength(100)] string LastName
);