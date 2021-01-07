using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class GameManager : MonoBehaviour
    {
        public int maxHeight = 15;
        public int maxWidth = 17;

        public Color color1;
        public Color color2;
        public Color appleColor = Color.red;
        public Color playerColor = Color.black;

        public Transform cameraHolder;
        
        private GameObject mapObject;
        private SpriteRenderer mapRenderer;

        private GameObject playerObj;
        private GameObject appleObj;
        private Node playerNode;
        private Node appleNode;

        private Node[,] grid;
        private List<Node> availableNodes = new List<Node>();

        private bool up, left, right, down;

        public float moveRate = 0.5f;
        private float timer;
        
        private Direction currentDirection;
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        
        // Start is called before the first frame update
        private void Start()
        {
            CreateMap();
            PlacePlayer();
            CreateApple();
            PlaceCamera();
            currentDirection = Direction.Right;
        }

        #region INIT
        private void CreateMap()
        {
            mapObject = new GameObject("Map");
            mapRenderer = mapObject.AddComponent<SpriteRenderer>();

            grid = new Node[maxWidth, maxHeight];
            
            Texture2D txt = new Texture2D(maxWidth, maxHeight);
            
            for (int x = 0; x < maxWidth; x++)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    Vector3 gridPos = Vector3.zero;
                    gridPos.x = x;
                    gridPos.y = y;
                    
                    Node n = new Node()
                    {
                        x = x,
                        y = y,
                        worldPosition = gridPos
                    };
                    grid[x, y] = n;
                    availableNodes.Add(n);
                    
                    #region VISUAL
                    if (x % 2 != 0)
                    {
                        if (y % 2 != 0)
                        {
                            txt.SetPixel(x, y, color1);
                        }
                        else
                        {
                            txt.SetPixel(x, y, color2);
                        }
                    }
                    else
                    {
                        if (y % 2 != 0)
                        {
                            txt.SetPixel(x, y, color2);
                        }
                        else
                        {
                            txt.SetPixel(x, y, color1);
                        }
                    }
                    #endregion
                }
            }

            txt.filterMode = FilterMode.Point;
            txt.Apply();
            Rect rect = new Rect(0, 0, maxWidth, maxHeight);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }

        private void PlacePlayer()
        {
            playerObj = new GameObject("Player");
            SpriteRenderer playerRender = playerObj.AddComponent<SpriteRenderer>();
            playerRender.sprite = CreateSprite(playerColor);
            playerRender.sortingOrder = 1;

            playerNode = GetNode(3, 3);
            playerObj.transform.position = playerNode.worldPosition;
        }

        private void PlaceCamera()
        {
            Node n = GetNode(maxWidth / 2, maxHeight / 2);
            Vector3 p = n.worldPosition;
            p += Vector3.one * 0.5f;

            cameraHolder.position = p;
        }

        private void CreateApple()
        {
            appleObj = new GameObject("Apple");
            SpriteRenderer appleRenderer = appleObj.AddComponent<SpriteRenderer>();
            appleRenderer.sprite = CreateSprite(appleColor);
            appleRenderer.sortingOrder = 1;
            RandomlyPlaceApple();
        }
        #endregion

        #region UPDATE
        private void Update()
        {
            GetInput();
            SetPlayerDirection();

            timer += Time.deltaTime;
            if (timer > moveRate)
            {
                timer = 0.0f;
                MovePlayer();
            }
            
        }
        
        private void GetInput()
        {
            up = Input.GetButtonDown("Up");
            down = Input.GetButtonDown("Down");
            left = Input.GetButtonDown("Left");
            right = Input.GetButtonDown("Right");
        }

        private void SetPlayerDirection()
        {
            if (up)
            {
                currentDirection = Direction.Up;
            }
            else if (down)
            {
                currentDirection = Direction.Down;
            }
            else if (left)
            {
                currentDirection = Direction.Left;
            }
            else if (right)
            {
                currentDirection = Direction.Right;
            }
        }

        private void MovePlayer()
        {
            int x = 0;
            int y = 0;
            
            switch (currentDirection)
            {
                case Direction.Up:
                    y = 1;
                    break;
                case Direction.Down:
                    y = -1;
                    break;
                case Direction.Left:
                    x = -1;
                    break;
                case Direction.Right:
                    x = 1;
                    break;
            }

            Node targetNode = GetNode(playerNode.x + x, playerNode.y + y);
            if (targetNode == null)
            {
                //Game Over
            }
            else
            {
                bool isScore = false;
                
                if (targetNode == appleNode)
                {
                    // You've Scored
                    isScore = true;
                    
                    if (availableNodes.Count == 0)
                    {
                        // You've Won!
                    }
                }

                availableNodes.Add(playerNode);
                playerObj.transform.position = targetNode.worldPosition;
                playerNode = targetNode;
                availableNodes.Remove(playerNode);

                //Move Tail
                
                if (isScore)
                {
                    if (availableNodes.Count > 0)
                    {
                        RandomlyPlaceApple();
                    }
                    else
                    {
                        // You've Won!
                    }
                }
            }
        }
        #endregion
        
        #region UTILITIES
        private Node GetNode(int x, int y)
        {
            if (x < 0 || x > maxWidth - 1 || y < 0 || y > maxHeight - 1)
            {
                return null;
            }
            return grid[x, y];
        }

        private Sprite CreateSprite(Color targetColor)
        {
            Texture2D txt = new Texture2D(1, 1);
            txt.SetPixel(0, 0, targetColor);
            txt.Apply();
            txt.filterMode = FilterMode.Point;
            Rect rect = new Rect(0, 0, 1, 1);
            return Sprite.Create(txt, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
        }

        private void RandomlyPlaceApple()
        {
            int random = Random.Range(0, availableNodes.Count);
            Node n = availableNodes[random];
            appleObj.transform.position = n.worldPosition;
            appleNode = n;
        }
        #endregion
    }
}
