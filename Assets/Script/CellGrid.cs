using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
    private readonly Cell[,] cells;

    public int Width => cells.GetLength(0);

    public int Height => cells.GetLength(1);

    public Cell this[int x,int y] => cells[x,y];

    public List<Cell> bombCells = new List<Cell>();
    public List<Cell> FlaggedCells = new List<Cell>();
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
        bombCells.Clear();
        for (int i = 0; i < numberOfMines; i++)
        {
            int x = Random.Range(0, Width);
            int y = Random.Range(0, Height);

            Cell cell = cells[x, y];
            if (cell.type != CELL_TYPE.MINE && !IsAdjacent(cell,startingCell))
            {
                cells[x, y].type = CELL_TYPE.MINE;
                bombCells.Add(cells[x, y]);
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

    private void FloodEmptyCell(Cell? _cell)
    {
        if (_cell == null) return;
        if (!InBounds(_cell.Value.cellPosition)) return;
        if (_cell.Value.isRevealed) return; ;
        if (_cell.Value.isFlagged || _cell.Value.type == CELL_TYPE.MINE) return;

        cells[_cell.Value.cellPosition.x, _cell.Value.cellPosition.y].isRevealed = true;
        if(_cell.Value.type == CELL_TYPE.EMPTY)
        {
            if (TryGetCell(new Vector3Int(_cell.Value.cellPosition.x + 1, _cell.Value.cellPosition.y),out Cell? left))
            {
                FloodEmptyCell(left);
            }
            if (TryGetCell(new Vector3Int(_cell.Value.cellPosition.x - 1, _cell.Value.cellPosition.y), out Cell? right))
            {
                FloodEmptyCell(right);
            }
            if (TryGetCell(new Vector3Int(_cell.Value.cellPosition.x, _cell.Value.cellPosition.y+1), out Cell? Up))
            {
                FloodEmptyCell(Up);
            }
            if (TryGetCell(new Vector3Int(_cell.Value.cellPosition.x + 1, _cell.Value.cellPosition.y-1), out Cell? Down))
            {
                FloodEmptyCell(Down);
            }
        }
    }

    public void RevealCell(Vector3Int gridPos)
    {
        Cell? cell = GetCellAtPosition(gridPos);
        if (cell!=null && InBounds(gridPos) && !cell.Value.isFlagged && !cell.Value.isRevealed)
        {
            if (cell.Value.type == CELL_TYPE.EMPTY)
            {
                FloodEmptyCell((Cell)cell);
            }
            else
            {
                cells[gridPos.x,gridPos.y].isRevealed = true;
            }
        }

        if (cell.Value.type == CELL_TYPE.MINE)
        {
            cells[gridPos.x, gridPos.y].isExploded = true;
        }
    }

    public void FlagCell(Vector3Int gridPos)
    {
        Cell? cell = GetCellAtPosition(gridPos);
        if (InBounds(gridPos) && cell != null && !cell.Value.isRevealed)
        {
            cells[gridPos.x, gridPos.y].isFlagged = !cells[gridPos.x, gridPos.y].isFlagged;
            if (cells[gridPos.x, gridPos.y].isFlagged)
            {
                FlaggedCells.Add(cells[gridPos.x, gridPos.y]);
            }
            else
            {
                Cell _cell = cells[gridPos.x, gridPos.y];
                if (FlaggedCells.Contains(_cell))
                {
                    FlaggedCells.Remove(_cell);
                }
            }
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

    public bool TryGetCell(Vector3Int pos ,out Cell? cell)
    {
        Cell? cell1 = GetCellAtPosition(pos);
        if (cell1 != null)
        {
            cell = cell1;
        }
        else
        {
            cell = null;
        }

        
        return cell != null;
    }


    public void RevealMineCell(int x, int y) => cells[x, y].isRevealed = true;

    public void RevealWrongFlaggedCells()
    {
        foreach(Cell cell in FlaggedCells)
        {
            if(cell.type != CELL_TYPE.MINE)
            {
                cells[cell.cellPosition.x, cell.cellPosition.y].type = CELL_TYPE.INCORRECT_GUESS;
            }
        }
    }
}
