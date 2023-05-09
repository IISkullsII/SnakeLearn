using UnityEngine;

namespace _02_Scripts
{
    public class GameController : MonoBehaviour
    {

        [SerializeField] private GameObject snakePlayer;
        [SerializeField] private GameObject snakeFood;

        [SerializeField] private int roomWidthCol = 16;
        [SerializeField] private int roomHeightRow = 9;
        [SerializeField] private float cellSize = 1f;
        public float CellSize => cellSize;

        [SerializeField] private float borderSize = .25f;
        [SerializeField] private Color borderColor = Color.white;
    
        [SerializeField] private bool drawDebugGrid = false;

        private void OnDrawGizmos()
        {
            if (!this.drawDebugGrid) return;
        
            Gizmos.color = Color.cyan;
        
        
            // Drawing vertical Lines
            float topPos = -(roomHeightRow * cellSize / 2);
            float bottomPos = -topPos;
        
            for (int i = 0; i < this.roomWidthCol; i++)
            {
                float widthPos = -(roomWidthCol * cellSize / 2);
                widthPos += i * cellSize;

                Gizmos.DrawLine(new Vector3(widthPos, topPos), new Vector3(widthPos, bottomPos));
            }
        
            float leftPos = -(roomWidthCol * cellSize / 2);
            float rightPos = -leftPos;

            for (int i = 0; i < this.roomHeightRow; i++)
            {
                float heightPos = -(roomHeightRow * cellSize / 2);
                heightPos += i * cellSize;
            
                Gizmos.DrawLine(new Vector3(leftPos, heightPos), new Vector3(rightPos, heightPos));
            }
        }
    }
}
