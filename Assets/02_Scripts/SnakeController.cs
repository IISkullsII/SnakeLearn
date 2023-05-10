using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

namespace _02_Scripts
{
    public class SnakeController : MonoBehaviour
    {
        [SerializeField] private GameObject snakeBodyPart;
        private SnakeBody _snakeBody;

        [SerializeField] private float snakeSpeed = 1f;     // Factor for timeTillMove
        [SerializeField] private float timeTillMove = 1f;   // In Seconds
        [SerializeField] private int snakeBodyStart = 3;
        
        private Vector3 _snakeDirection = Vector3.up;

        private bool _isDead = false;
        public bool IsDead => _isDead;

        [SerializeField] private GameObject gameControllerObject;
        private GameController _gameController;

        private bool _inputLock;

        [SerializeField] private bool debugDrawDirection;

        private void Awake()
        {
            _gameController = gameControllerObject.GetComponent<GameController>();
        
        }

    
        private void Start()
        {
            _snakeBody = new SnakeBody(snakeBodyPart, gameObject.transform, snakeBodyStart);
            
            StartCoroutine(MoveSnake());
        }
    
    
        private void Update()
        {
            ChangeDirection();

            if (Input.GetKeyDown(KeyCode.F))
            {
                _snakeBody.AddBodyPart();
            }
        }

        public void ResetSnake()
        {
            _snakeBody.ResetBody(snakeBodyStart);
            transform.position = new Vector3(_gameController.CellSize / 2, _gameController.CellSize / 2, 0);
            _isDead = false;
            StartCoroutine(MoveSnake());
        }

    
        private void ChangeDirection()
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            if ((inputX == 0 && inputY == 0) || _inputLock)
            {
                return;
            }
        
            _inputLock = true;

            _snakeDirection = inputX switch
            {
                < 0 when _snakeDirection != Vector3.right => Vector3.left,
                > 0 when _snakeDirection != Vector3.left => Vector3.right,
                _ => _snakeDirection
            };
        
            _snakeDirection = inputY switch
            {
                > 0 when _snakeDirection != Vector3.down => Vector3.up,
                < 0 when _snakeDirection != Vector3.up => Vector3.down,
                _ => _snakeDirection
            };
        }

        private IEnumerator MoveSnake()
        {
            while (!_isDead)
            {
                Vector3 transformPos = transform.position;
                _snakeBody.MoveBodyParts(transformPos);
                
                float movementMagnitude = _gameController.CellSize;

                Vector3 nextPosition = transformPos + (_snakeDirection * movementMagnitude);
                
                if (CheckCollision(nextPosition))
                {
                    _isDead = true;
                    _gameController.TriggerGameOver();
                    yield return null;
                }

                int eatFood = CheckFoodCollision(nextPosition);
                if (eatFood >= 0)
                {
                    _gameController.EatFood(eatFood);
                    _snakeBody.AddBodyPart();
                }

                transform.position += _snakeDirection * movementMagnitude;

                if (debugDrawDirection)
                {   
                
                    Debug.DrawLine(transform.position, transformPos + (_snakeDirection * (movementMagnitude * 2)), Color.green, timeTillMove / snakeSpeed);
                }

                _inputLock = false;
            
                yield return new WaitForSeconds(timeTillMove / snakeSpeed);   
            }
            yield return null;
        }


        private bool CheckCollision(Vector3 nextPosition)
        {
            return CheckOutOfBounds(nextPosition) || CheckSelfCollision(nextPosition);
        }

        private bool CheckOutOfBounds(Vector3 nextPosition)
        {
            if (nextPosition.x > (_gameController.RoomWidth / 2) || nextPosition.x < -(_gameController.RoomWidth / 2))
                return true;

            if (nextPosition.y > (_gameController.RoomHeight / 2) || nextPosition.y < -(_gameController.RoomHeight / 2))
                return true;

            return false;
        }
        
        private bool CheckSelfCollision(Vector3 nextPosition)
        {
            for (int i = 0; i < _snakeBody.SnakeLength; i++)
            {
                if (nextPosition == _snakeBody.BodyParts[i].Position) return true;
            }
            
            return false;
        }


        private int CheckFoodCollision(Vector3 nextPosition)
        {
            List<GameObject> listFood = _gameController.ListFood;
            for (int i = 0; i < listFood.Count; i++)
            {
                if (nextPosition == listFood[i].transform.position)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public class SnakeBody
    {
        private readonly GameObject _bodyPartPrefab;
        private readonly Transform _snakeBodyParent;
        
        private readonly List<BodyPart> _bodyParts = new List<BodyPart>();
        public int SnakeLength => _bodyParts.Count;
        public List<BodyPart> BodyParts => _bodyParts;

        public SnakeBody(GameObject bodyPartPrefab, Transform snakeBodyParent, int startLength = 3)
        {
            this._bodyPartPrefab = bodyPartPrefab;
            this._snakeBodyParent = snakeBodyParent;

            InitBody(startLength);
        }

        public void ResetBody(int startLength)
        {
            ClearBodyParts();
            InitBody(startLength);
        }

        private void InitBody(int startLength)
        {
            for (int i = 0; i < startLength; i++)
            {
                AddBodyPart();
            }
        }

        public void AddBodyPart()
        {
            Vector3 lastBodyPartPosition = _bodyParts.Count > 0 ? _bodyParts[^1].Position : _snakeBodyParent.position;
            BodyPart newBodyPart = new(_bodyPartPrefab, lastBodyPartPosition);
            _bodyParts.Add(newBodyPart);
        }

        private void ClearBodyParts()
        {
            foreach (BodyPart t in _bodyParts)
            {
                t.DestroyBodyPart();
            }
            _bodyParts.Clear();
        }

        public void MoveBodyParts(Vector3 headPosition)
        {
            for (int i = _bodyParts.Count - 1; i > 0; i--)
            {
                Vector3 previousPartPosition = _bodyParts[i - 1].Position;
                _bodyParts[i].Position = previousPartPosition;
            }

            _bodyParts[0].Position = headPosition;
        }
    }
    

    public class BodyPart
    {
        private readonly GameObject _bodyObject;

        public Vector3 Position
        {
            get => _bodyObject.transform.position;
            set => _bodyObject.transform.position = value;
        }
        public BodyPart(GameObject bodyPartPrefab, Vector3 transformPos)
        {
            this._bodyObject = Object.Instantiate(bodyPartPrefab, transformPos, Quaternion.identity);
        }

        public void DestroyBodyPart()
        {
            Object.Destroy(_bodyObject);
        }
    }
}
