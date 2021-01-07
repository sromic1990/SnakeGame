using System;
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
        public Color playerColor = Color.black;

        public Transform cameraHolder;
        
        private GameObject mapObject;
        private SpriteRenderer mapRenderer;

        private GameObject playerObj;
        private Node playerNode;

        private Node[,] grid;

        private bool up, left, right, down;
        private bool movePlayer;
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
            PlaceCamera();
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
        #endregion

        #region UPDATE
        private void Update()
        {
            GetInput();
            SetPlayerDirection();
            MovePlayer();
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
                movePlayer = true;
            }
            else if (down)
            {
                currentDirection = Direction.Down;
                movePlayer = true;
            }
            else if (left)
            {
                currentDirection = Direction.Left;
                movePlayer = true;
            }
            else if (right)
            {
                currentDirection = Direction.Right;
                movePlayer = true;
            }
        }

        private void MovePlayer()
        {
            if(!movePlayer)
                return;
            
            movePlayer = false;
            
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
                playerObj.transform.position = targetNode.worldPosition;
                playerNode = targetNode;
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
        #endregion
    }
}
