using System;
using System.Collections.Generic;

namespace ArpellaStores.Features.SmsManagement.Models;

public partial class SmsTemplate
{
    public int Id { get; set; }

    public string TemplateType { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
