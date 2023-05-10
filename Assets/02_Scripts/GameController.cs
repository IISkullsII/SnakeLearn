using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02_Scripts
{
    public class GameController : MonoBehaviour
    {

        [SerializeField] private GameObject snakePlayer;
        private SnakeController _snakePlayerController;

        [SerializeField] private GameObject snakeFoodPrefab;

        [SerializeField] private int roomWidthCol = 16;
        [SerializeField] private int roomHeightRow = 9;

        [SerializeField] private GameObject deadUIObject;

        public int RoomWidthCol => roomWidthCol;
        public float RoomWidth => roomWidthCol * cellSize;
        public int RoomHeightCol => roomHeightRow;
        public float RoomHeight => roomHeightRow * cellSize;
        
        [SerializeField] private float cellSize = 1f;
        public float CellSize => cellSize;

        [SerializeField] private float borderSize = .25f;
        [SerializeField] private Color borderColor = Color.white;
    
        [SerializeField] private bool drawDebugGrid = false;

        [SerializeField] private int amountFood = 1;
        private List<GameObject> _listFood = new List<GameObject>();
        public List<GameObject> ListFood => _listFood;

        [SerializeField] private GameObject uiCanvas;
        private TextFadeController _textFadeController;

        private void Awake()
        {
            _textFadeController = uiCanvas.GetComponentInChildren<TextFadeController>();
        }

        private void Start()
        {
            _snakePlayerController = snakePlayer.GetComponent<SnakeController>();
            InitFoodList();
        }

        public void TriggerGameOver()
        {
            uiCanvas.SetActive(true);
            _textFadeController.StartFade();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                SpawnNewFood();
            }

            if (Input.GetKeyDown(KeyCode.Space) && _snakePlayerController.IsDead)
            {
                ResetGame();
            }
        }

        public void EatFood(int listFoodIndex)
        {
            if (listFoodIndex < 0 && listFoodIndex >= _listFood.Count) return;

            GameObject eatenFood = _listFood[listFoodIndex];
            _listFood.RemoveAt(listFoodIndex);
            Destroy(eatenFood);

            if (_listFood.Count < amountFood)
            {
                SpawnNewFood();
            }
        }

        private void InitFoodList()
        {
            for (int i = 0; i < amountFood; i++)
            {
                SpawnNewFood();
            }
        }

        public void SpawnNewFood()
        {
            Vector3 newPos = GetRandomPointInRoom();
            
            GameObject newFood = Instantiate(snakeFoodPrefab, newPos, Quaternion.Euler(90f, 0f, 0f));
            _listFood.Add(newFood);
        }

        private void ClearFoodList()
        {
            foreach (GameObject t in _listFood)
            {
                Destroy(t);
            }

            _listFood.Clear();
        }

        private Vector3 GetRandomPointInRoom()
        {
            Vector3 newPos = Vector3.zero;
            newPos.x = Random.Range(0, roomWidthCol) * cellSize + (cellSize / 2) - (RoomWidth / 2);
            newPos.y = Random.Range(0, roomHeightRow) * cellSize + (cellSize / 2) - (RoomHeight / 2);

            return newPos;
        }

        private void ResetGame()
        {
            uiCanvas.SetActive(false);
            ClearFoodList();
            InitFoodList();
            _snakePlayerController.ResetSnake();
        }
        
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
