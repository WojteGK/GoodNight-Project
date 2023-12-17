using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace GoodNightProject.Droid
{
    public interface IAlgorithm
    {
        (int, int) algorithm(int hour, int minute);
    }
}