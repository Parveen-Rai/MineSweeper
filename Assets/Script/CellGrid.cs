using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
    private readonly Cell[,] cells;

    public int Width => cells.GetLength(0);

    public int Height => cells.GetLength(1);

    public Cell this[int x,int y] => cells[x,y];


    public CellGrid(int _width, int _height)
    {
        cells = new Cell[_width, _height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Cell cell = new Cell();
                cell.cellPosition = new Vector3Int(x, y, 0);
                cell.type = CELL_TYPE.EMPTY;
                cell.isRevealed = false;
                cells[x, y] = cell;
            }
        }
    }

    public void PlaceMines(Cell startingCell, int numberOfMines)
    {
        for (int i = 0; i < numberOfMines; i++)
        {
            int x = Random.Range(0, Width);
            int y = Random.Range(0, Height);

            Cell cell = cells[x, y];
            if (cell.type != CELL_TYPE.MINE && !IsAdjacent(cell,startingCell))
            {
                cells[x, y].type = CELL_TYPE.MINE;
            }
            else
            {
                i--;
            }
        }
    }

    public void PlacingNumbers()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Cell cell = cells[x, y];
                if (cell.type != CELL_TYPE.MINE)
                {
                    int count = CalCulateAdjacentMines(x, y);
                    if (count == 0) continue;
                    cells[x, y].type = CELL_TYPE.NUMBER;
                    cells[x, y].number = count;
                }

            }
        }
    }

    private int CalCulateAdjacentMines(int x, int y)
    {
        int mineCount = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int checkX = x + i;
                int checkY = y + j;

                if (checkX >= 0 && checkX < Width && checkY >= 0 && checkY < Height)
                {
                    if (cells[checkX, checkY].type == CELL_TYPE.MINE)
                    {
                        mineCount++;
                    }
                }
            }
        }
        return mineCount;
    }

    private void FloodEmptyCell(Vector3Int gridPos)
    {
        Queue<Cell> cellsToCheck = new Queue<Cell>();
        Cell? startCell = GetCellAtPosition(gridPos);
        cellsToCheck.Enqueue((Cell)startCell);
        while (cellsToCheck.Count > 0)
        {
            Cell cell = cellsToCheck.Dequeue();
            if (!cell.isRevealed && !cell.isFlagged)
            {
                cells[cell.cellPosition.x, cell.cellPosition.y].isRevealed = true;
                if (cell.number == 0)
                {
                    List<Cell> celList = GetNeighbors(cell);
                    foreach (Cell _cell in celList)
                    {
                        if (!_cell.isRevealed && !cell.isFlagged)
                        {
                            cellsToCheck.Enqueue(_cell);
                        }
                    }
                }
            }
        }
    }

    public void RevealCell(Vector3Int gridPos)
    {
        Cell cell = (Cell)GetCellAtPosition(gridPos);
        if (InBounds(gridPos) && !cell.isFlagged && !cell.isRevealed)
        {
            if (cell.type == CELL_TYPE.EMPTY)
            {
                FloodEmptyCell(gridPos);
            }
            else
            {
                cells[gridPos.x,gridPos.y].isRevealed = true;
            }
        }

    }

    public void FlagCell(Vector3Int gridPos)
    {
        Cell? cell = GetCellAtPosition(gridPos);
        if (InBounds(gridPos) && cell != null && !cell.Value.isRevealed)
        {
            cells[gridPos.x, gridPos.y].isFlagged = !cells[gridPos.x, gridPos.y].isFlagged;
        }
    }

    private bool IsAdjacent(Cell a, Cell b)
    {
        return Mathf.Abs(a.cellPosition.x - b.cellPosition.x) <= 1 && Mathf.Abs(a.cellPosition.y - b.cellPosition.y) <=1;
    }

    public bool InBounds(Vector3Int position)
    {
        if (position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height)
        {
            return true;
        }
        return false;
    }

    public Cell? GetCellAtPosition(Vector3Int position)
    {
        if (InBounds(position))
        {
            return cells[position.x, position.y];
        }
        else
        {
            return null;
        }
    }

    List<Cell> GetNeighbors(Cell Cell)
    {
        List<Cell> neighbors = new List<Cell>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                Vector3Int neighborPos = new Vector3Int(Cell.cellPosition.x + dx, Cell.cellPosition.y + dy, Cell.cellPosition.z);
                Cell? neigborcell = GetCellAtPosition(neighborPos);
                if (neigborcell != null)
                {
                    neighbors.Add((Cell)neigborcell);
                }

            }
        }

        return neighbors;
    }
}
