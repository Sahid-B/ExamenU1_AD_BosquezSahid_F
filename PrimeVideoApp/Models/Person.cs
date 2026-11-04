using System;
using System.Collections.Generic;

namespace PrimeVideoApp.Models;

public partial class Person
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ContentPerson> ContentPeople { get; set; } = new List<ContentPerson>();
}
