using UnityEngine;
using System.Collections;

namespace ActionGameLibrary
{
    public interface ICombo
    {
        void ReceivedCombo(string combo, bool isSimulation = false);
        void ReceivedJoyData(float angle, float power, bool isSimulation = false);
    }
}
