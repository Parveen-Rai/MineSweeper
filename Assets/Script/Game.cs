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
    public OnBoardUpdate boardUpdate;
    void Start()
    {
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        grid = new CellGrid(width, height);
        board.Draw(grid);
        boardUpdate = () => board.Draw(grid);
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
        Vector3Int gridPos = board.tilemap.WorldToCell(pos);
        if (Input.GetMouseButtonDown(0))
        {
            Reveal(gridPos);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Flag(gridPos);
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
            StartCoroutine(RevealMines());
        }
      
    }

    public IEnumerator RevealMines()
    {
        for (int i = 0; i < grid.bombCells.Count; i++)
        {
            yield return new WaitForSeconds(0.2f);
            if (!grid.bombCells[i].isExploded || !grid.bombCells[i].isFlagged)
            {
                grid.RevealMineCell(grid.bombCells[i].cellPosition.x, grid.bombCells[i].cellPosition.y);
                board.Draw(grid);
            }
        }
    }
}
