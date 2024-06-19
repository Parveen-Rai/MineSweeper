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
    void Start()
    {
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        grid = new CellGrid(width,height);
        board.Draw(grid);
    }

    void Update()
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
            Cell Cell = (Cell)grid.GetCellAtPosition(pos);
            grid.PlaceMines(Cell,numberOfMines);
            grid.PlacingNumbers();
            grid.RevealCell(pos);
            isGenerated = true;
        }
        board.Draw(grid);
    }
}
