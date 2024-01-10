using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoodNightProject
{
    public interface IAlarmService
    {
        void SetAlarm(int hour, int minute);
        void CancelAlarm();
        void CancelMedia();

    }
}
