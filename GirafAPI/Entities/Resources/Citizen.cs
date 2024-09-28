﻿using GirafAPI.Entities.Weekplans;

namespace GirafAPI.Entities.Resources;

public class Citizen
{
    public int Id { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public int WeekplanId { get; set; }
    
    public Weekplan? Weekplan { get; set; }
}