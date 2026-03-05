using UnityEngine;
using System.Collections;

namespace ActionGameLibrary
{
    public interface IDoCombo
    {

        void ReserveCombo(ICombo ic);

        void UnreserveCombo(ICombo ic);

        void NoticeCombo(string combo);
    }
}