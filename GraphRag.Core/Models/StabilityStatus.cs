using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

public enum StabilityStatus
{
    Identical,

    Updated,

    Split,

    Merged,

    Deleted,

    New
}
