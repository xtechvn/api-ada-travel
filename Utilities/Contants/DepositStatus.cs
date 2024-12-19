using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Contants
{
    public enum DepositStatus
    {
        NEW_TRANSACTION =0,
        WAITING_PAYMENT=1,
        WAITING_CONFIRMATION=2,
        ACCEPTED=3,
        REJECTED=4

    }
}
