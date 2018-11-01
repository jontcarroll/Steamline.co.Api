using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.Utils.Cron
{
    [Serializable]
    public enum CrontabFieldType
    {
        Minute,
        Hour,
        Day,
        Month,
        DayOfWeek
    }
}
