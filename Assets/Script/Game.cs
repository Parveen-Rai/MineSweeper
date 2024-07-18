using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Board board;
    public int width = 9;
    public int height = 9;
    public int numberOfMines = 10;
    public float longPressDuration = 1f;
    private float touchStartTime;
    private bool longPressTriggered;
    private CellGrid grid;
    private bool isGenerated = false;
    private bool isGameOver = false;
    private Vector3 touchStart = Vector3.zero;

    public CameraController CameraController;
    void Start()
    {
       CameraController.init(width,height);
        grid = new CellGrid(width, height);
        board.Draw(grid);
    }

    void Update()
    {

        if (!isGameOver)
        {
            TouchControls();
            MouseControls();
        }
      
    }

    private void TouchControls()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Vector3Int gridPos = board.tilemap.WorldToCell(touchPos);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartTime = Time.time;
                    longPressTriggered = false;
                    break;
                case TouchPhase.Stationary:
                case TouchPhase.Moved:
                    if (!longPressTriggered && Time.time - touchStartTime >= longPressDuration)
                    {
                        longPressTriggered = true;
                        Flag(gridPos);
                    }
                    break;
                case TouchPhase.Ended:
                    if (!longPressTriggered)
                    {
                        Reveal(gridPos);
                    }
                    break;
            }
        }
    }

    private void MouseControls()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            touchStart = Input.mousePosition;
        }
        Vector3Int gridPos = board.tilemap.WorldToCell(pos);
        if(Input.mousePosition == touchStart)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Reveal(gridPos);
                touchStart = Vector3.zero;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Flag(gridPos);
                touchStart = Vector3.zero;
            }
           
        }
    }

    private void Flag(Vector3Int pos)
    {
        grid.FlagCell(pos);
        board.Draw(grid);
    }

    private void Reveal(Vector3Int pos)
    {
        if (isGenerated)
        {
            grid.RevealCell(pos);
        }
        else
        {
            Cell? Cell = grid.GetCellAtPosition(pos);
            if (Cell != null && grid.InBounds(pos))
            {
                grid.PlaceMines(Cell.Value, numberOfMines);
                grid.PlacingNumbers();
                grid.RevealCell(pos);
                isGenerated = true;
            }
           
        }
        board.Draw(grid);
        IsBombRevealed(pos);
    }

    public bool CheckWin()
    {
        return false;
    }

    public void IsBombRevealed(Vector3Int pos)
    {
        Cell? Cell = grid.GetCellAtPosition(pos);
        if (Cell != null && grid.InBounds(pos) && Cell.Value.type == CELL_TYPE.MINE)
        {
            isGameOver = true;
            isGenerated = false;
            grid.RevealWrongFlaggedCells();
            StartCoroutine(RevealMines());
        }
      
    }

    public IEnumerator RevealMines()
    {
        for (int i = 0; i < grid.bombCells.Count; i++)
        {
            yield return new WaitForSeconds(0.2f);
            Cell cell = grid[grid.bombCells[i].cellPosition.x, grid.bombCells[i].cellPosition.y];
            if (!cell.isExploded && !cell.isFlagged)
            {
                grid.RevealMineCell(grid.bombCells[i].cellPosition.x, grid.bombCells[i].cellPosition.y);
                board.Draw(grid);
            }
        }
    }
}
