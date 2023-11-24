using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoodNightProject
{
    public interface IAlarmService
    {
        Task SetAlarm(int hour, int minute);
        Task CancelAlarm();

    }
}
