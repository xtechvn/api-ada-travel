using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Utilities
{
    public enum DebtType
    {
        [Description("Được công nợ")]
        DEBT_ACCEPTED = 1,

        [Description("Không được công nợ")]
        DEBT_NOT_ACCEPTED = 0,

    }
    public enum DebtStatus
    {
       NEW=0,
       PAID=1,
        PAID_NOT_ENOUGH = 2

    }
}
