using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Tile tilePrefab;
    public TileState[] tileStates;
    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting;

    private Vector2 touchStartPos;
    private float swipeThreshold = 100f; // Adjust this value for sensitivity
    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }
        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }
    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.SpawnTile(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    private void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }

        // ------ Mobile Touch Control ------ //
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Ended:
                    Vector2 touchEndPos = touch.position;
                    Vector2 swipeDirection = touchEndPos - touchStartPos;

                    if (swipeDirection.magnitude >= swipeThreshold)
                    {
                        float angle = Mathf.Atan2(swipeDirection.y, swipeDirection.x) * Mathf.Rad2Deg;

                        if (angle < 45f && angle > -45f)
                        {
                            MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
                           // Debug.Log("Swipe Right");
                        }
                        else if (angle >= 45f && angle < 135f)
                        {
                            MoveTiles(Vector2Int.up, 0, 1, 1, 1);
                           // Debug.Log("Swipe Up");
                        }
                        else if (angle >= 135f || angle <= -135f)
                        {
                            MoveTiles(Vector2Int.left, 1, 1, 0, 1);
                           // Debug.Log("Swipe Left");
                        }
                        else if (angle > -135f && angle < -45f)
                        {
                            MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
                           // Debug.Log("Swipe Down");
                        }
                    }
                    break;
            }
        }

    }

    public void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int i = startX; i >= 0 && i < grid.width; i += incrementX)
        {
            for (int j = startY; j >= 0 && j < grid.height; j += incrementY)
            {
                TileCell cell = grid.GetCell(i, j);
                if (cell.occupied)
                {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }
        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }
    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adajecent = grid.GetAdajecentCell(tile.cell, direction);

        while (adajecent != null)
        {
            if (adajecent.occupied)
            {
                if (CanMerge(tile, adajecent.tile))
                {
                    Merge(tile, adajecent.tile);
                    return true;
                }
                break;
            }
            newCell = adajecent;
            adajecent = grid.GetAdajecentCell(adajecent, direction);
        }
        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }
        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.locked;
    }

    private void Merge(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;
        b.SetState(tileStates[index], number);

        gameManager.IncreaseScore(number);
    }
    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }
        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;

        foreach (var tile in tiles)
        {
            tile.locked = false;
        }

        if (tiles.Count != grid.size)
        {
            CreateTile();
        }
        if (CheckForGameOver())
        {
            gameManager.GameOver();
        }
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdajecentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdajecentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdajecentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdajecentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile))
            {
                return false;
            }
            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }
            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }
            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
    }    

}
