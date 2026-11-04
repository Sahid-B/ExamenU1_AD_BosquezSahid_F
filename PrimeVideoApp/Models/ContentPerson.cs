using System;
using System.Collections.Generic;

namespace PrimeVideoApp.Models;

public partial class ContentPerson
{
    public int ContentId { get; set; }

    public int PersonId { get; set; }

    public string Role { get; set; } = null!;

    public int CreditOrder { get; set; }

    public virtual Content Content { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;
}
