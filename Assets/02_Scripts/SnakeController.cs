using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts
{
    public class SnakeController : MonoBehaviour
    {
        [SerializeField] private GameObject snakeBodyPart;
        private SnakeBody _snakeBody;

        [SerializeField] private float snakeSpeed = 1f;     // Factor for timeTillMove
        [SerializeField] private float timeTillMove = 1f;   // In Seconds
        
        private Vector3 _snakeDirection = Vector3.up;

        private bool _isDead = false;

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
            _snakeBody = new SnakeBody(snakeBodyPart, gameObject.transform, 3);
            
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
                _snakeBody.MoveBodyParts(transform.position);
                
                float movementMagnitude = _gameController.CellSize;

                if (CheckCollision(_snakeDirection * movementMagnitude))
                {
                    _isDead = true;
                    yield return null;
                }
                
                transform.position += _snakeDirection * movementMagnitude;

                if (debugDrawDirection)
                {   
                
                    Debug.DrawLine(transform.position, transform.position + (_snakeDirection * (movementMagnitude * 2)), Color.green, timeTillMove / snakeSpeed);
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
            // TODO: Check if nextPosition is outside of RoomBounds and return if true
            // Check NextX < -RoomWidth or NextX > RoomWidth
            // check NextY < -RoomHeight or Next > Roomheight

            return false;
        }
        
        private bool CheckSelfCollision(Vector3 nextPosition)
        {
            for (int i = 0; i < _snakeBody.SnakeLength; i++)
            {
                // TODO: Check if nextPosition of head is equal to the BodyPart and return if true
                // if (nextPosition.Equals(_snakeBody.BodyParts[i].Position)) return true;
            }
            return false;
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
        public BodyPart(GameObject bodyPartPrefab,Vector3 transformPos)
        {
            this._bodyObject = Object.Instantiate(bodyPartPrefab, transformPos, Quaternion.identity);
        }
    }
}
