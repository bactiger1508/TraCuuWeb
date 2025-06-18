using System;
using System.Collections.Generic;

namespace TraCuuWeb.Data;

public partial class PasswordResetToken
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public string Token { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool? IsUsed { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
