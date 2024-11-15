﻿using System.ComponentModel.DataAnnotations;

namespace GirafAPI.Entities.Citizens.DTOs;

// Data necessary to update a Citizen
public record UpdateCitizenDTO(
    [Required][StringLength(50)] string FirstName,
    [Required][StringLength(20)] string LastName
);