using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tile unknownTile;
    public Tile flagTile;
    public Tile WrongFlagged;
    public Tile mineTile;
    public Tile ExplodedTile;
    public Tile emptyTile;

    public Tile[] numberTile;


    public Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(CellGrid boardState)
    {
        int width = boardState.Width;
        int height = boardState.Height;
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j< height; j++)
            {
                Cell cell = boardState[i, j];
                tilemap.SetTile(cell.cellPosition,GetTile(cell));
            }
        }
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.isRevealed)
        {
            return GetRevealedTile(cell);
        }else if (cell.isFlagged)
        {
            return flagTile;
        }
        else
        {
            return unknownTile;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case CELL_TYPE.EMPTY:
                return emptyTile;
            case CELL_TYPE.MINE:
                if (cell.isExploded)
                {
                    return ExplodedTile;
                }
                else
                {
                    return mineTile;
                }
            case CELL_TYPE.NUMBER:
                return numberTile[cell.number-1];
            default:
                return null;
        }
    }

    
}
