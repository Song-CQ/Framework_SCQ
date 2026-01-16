using UnityEngine;

namespace ProjectApp
{
    //ÔªËØÀà
    public class Element : MonoBehaviour
    {
        public System.Action<int, int> OnElementClicked;

        private int gridX;
        private int gridY;
        private ElementType type;

        public void Initialize(int x, int y, ElementType elementType)
        {
            gridX = x;
            gridY = y;
            type = elementType;
        }

        public void UpdatePosition(int x, int y)
        {
            gridX = x;
            gridY = y;
        }

        void OnMouseDown()
        {
            OnElementClicked?.Invoke(gridX, gridY);
        }
    }
}
