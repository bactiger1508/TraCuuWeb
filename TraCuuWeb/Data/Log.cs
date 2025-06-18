using System;
using System.Collections.Generic;

namespace TraCuuWeb.Data;

public partial class Log
{
    public int Id { get; set; }

    public DateOnly LogDate { get; set; }

    public decimal? TotalRevenue { get; set; }

    public int? WebsiteVisits { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }
}
