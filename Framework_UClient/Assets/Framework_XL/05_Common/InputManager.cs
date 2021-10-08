
using System;
using UnityEngine;
using XL.Common;
namespace XL.XlInput
{
    public class InputManager : MonoBehaviour
    {
        public static event Action<Vector2> ClickScreen;

        public static event Action UpData;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ClickScreen?.Invoke(Input.mousePosition);
            }
            UpData?.Invoke();
        }

       


    }

}