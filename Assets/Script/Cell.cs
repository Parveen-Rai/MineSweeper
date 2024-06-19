using UnityEngine;

public struct Cell
{
    public CELL_TYPE type;
    public Vector3Int cellPosition;
    public int number;
    public bool isRevealed;
    public bool isFlagged;
    public bool isExploded;
}
