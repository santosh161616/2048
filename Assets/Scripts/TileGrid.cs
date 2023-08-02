using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows { get; private set; }
    public TileCell[] cells { get; private set; }
    public int size => cells.Length;
    public int height => rows.Length;
    public int width => size / height;

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    private void Start()
    {
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].cells.Length; x++)
            {
                rows[y].cells[x].cordinates = new Vector2Int(x, y);
            }
        }
    }
    public TileCell GetCell(int x, int y)
    {
        if(x >= 0 && x < width && y >= 0 && y < height)
        {
            return rows[y].cells[x];
        }
        else
        {
            return null;
        }
    }
    public TileCell GetCell(Vector2Int cordinates)
    {
        return GetCell(cordinates.x, cordinates.y);
    }
    public TileCell GetAdajecentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int cordinates = cell.cordinates;
        cordinates.x += direction.x;
        cordinates.y -= direction.y;
        return GetCell(cordinates);
    }

    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);
        int startingIndex = index;
        while (cells[index].occupied)
        {
            index++;
            if (index >= cells.Length)
            {
                index = 0;
            }
            if (index == startingIndex)
            {
                return null;
            }
        }
        return cells[index];
    }
}
