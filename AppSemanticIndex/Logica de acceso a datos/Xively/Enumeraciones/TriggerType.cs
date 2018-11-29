using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppSemanticIndex
{
    public enum TriggerType
    {
        gt,
        gte,
        lt,
        lte,
        eq,
        change,
        frozen,
        live,
    }
}
