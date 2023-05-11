using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private float offsetX = 0f;
        public float OffsetX => offsetX;
        [SerializeField] private float offsetY = 0f;
        public float OffsetY => offsetY;

        [SerializeField] private float borderSize = .25f;
        [SerializeField] private Color borderColor = Color.white;
    
        [SerializeField] private bool drawDebugGrid = false;

        [SerializeField] private int amountFood = 1;
        private List<GameObject> _listFood = new List<GameObject>();
        public List<GameObject> ListFood => _listFood;

        [SerializeField] private GameObject uiCanvas;
        private TextFadeController _textFadeController;
        
        private bool _gameStarted = false;

        [SerializeField] private GameObject uiGameStartedCanvas;

        [SerializeField] private GameObject scoreText;
        private TextMeshProUGUI _scoreTextMeshPro;
        [SerializeField] private float baseScoreAtEat = 150f;
        [SerializeField] private float timeScoreMin = 15F;
        [SerializeField] private float timeScoreMax = 100f;
        [SerializeField] private float timeMin = 10f; // In seconds
        [SerializeField] private float timeMax = 1f; // In Seconds
        private float _playerScore = 0;
        private System.DateTime _timeSinceEat;

        private void Awake()
        {
            _snakePlayerController = snakePlayer.GetComponent<SnakeController>();
            _textFadeController = uiCanvas.GetComponentInChildren<TextFadeController>();
            _scoreTextMeshPro = scoreText.GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            InitFoodList();
            _playerScore = 0;
        }

        public void TriggerGameOver()
        {
            uiCanvas.SetActive(true);
            _textFadeController.StartFade();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!_gameStarted)
                {
                    uiGameStartedCanvas.SetActive(false);
                    _gameStarted = true;
                    _snakePlayerController.ResetSnake();
                    _timeSinceEat = System.DateTime.UtcNow;
                    _playerScore = 0;
                    UpdateScoreText();
                }
                if (_snakePlayerController.IsDead)
                {
                    ResetGame();
                }
            }
        }

        public void UpdateScoreText()
        {
            _scoreTextMeshPro.text = "SCORE: " + Mathf.Round(_playerScore);
        }

        public void EatFood(int listFoodIndex)
        {
            if (listFoodIndex < 0 && listFoodIndex >= _listFood.Count) return;

            _playerScore += baseScoreAtEat;

            System.TimeSpan timeSpan = System.DateTime.UtcNow - _timeSinceEat;
            float sumTime = timeSpan.Seconds + (timeSpan.Milliseconds / 1000f);

            if (timeSpan.Seconds < timeMax)
            {
                _playerScore += timeScoreMax;
            }else if (timeSpan.Seconds > timeMin)
            {
                _playerScore += timeScoreMin;
            }
            else
            {
                _playerScore += Mathf.Lerp(timeScoreMax, timeScoreMin, sumTime / timeMin);
            }
            UpdateScoreText();

            GameObject eatenFood = _listFood[listFoodIndex];
            _listFood.RemoveAt(listFoodIndex);
            Destroy(eatenFood);

            if (_listFood.Count < amountFood)
            {
                SpawnNewFood();
            }

            _timeSinceEat = System.DateTime.UtcNow;
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
            newPos += new Vector3(offsetX, offsetY, 0);
            
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
            while (true)
            {
                Vector3 newPos = Vector3.zero;

                newPos.x = Random.Range(0, roomWidthCol) * cellSize + (cellSize / 2) - (RoomWidth / 2);
                newPos.y = Random.Range(0, roomHeightRow) * cellSize + (cellSize / 2) - (RoomHeight / 2);
                
                if (_snakePlayerController.CheckIfSnakeCollision(newPos, true))
                {
                    continue;
                }

                return newPos;
                break;
            }
        }

        private void ResetGame()
        {
            uiCanvas.SetActive(false);
            ClearFoodList();
            InitFoodList();
            _snakePlayerController.ResetSnake();
            _playerScore = 0;
            _timeSinceEat = System.DateTime.UtcNow;
            UpdateScoreText();
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

                Gizmos.DrawLine(new Vector3(widthPos + offsetX, topPos + offsetY), new Vector3(widthPos + offsetX, bottomPos + offsetY));
            }
        
            float leftPos = -(roomWidthCol * cellSize / 2);
            float rightPos = -leftPos;

            for (int i = 0; i < this.roomHeightRow; i++)
            {
                float heightPos = -(roomHeightRow * cellSize / 2);
                heightPos += i * cellSize;
            
                Gizmos.DrawLine(new Vector3(leftPos + offsetX, heightPos + offsetY), new Vector3(rightPos + offsetX, heightPos + offsetY));
            }
        }
    }
}
