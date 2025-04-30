using System;
using System.Collections.Generic;

namespace GitCRUDAPI.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }
}
