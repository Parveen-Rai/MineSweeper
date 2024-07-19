using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public Vector3 panLimit;
    public float scrollSpeed = 20f;
    public float minZoom = 5f;
    public float maxZoom = 50f;

    private Vector3 touchStart;

    private Vector3 startPos;

    private Vector3 currentPos;
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;

    private Vector2 boardSize;
    private Vector2 minCameraClamp;
    private Vector2 maxCameraClamp;


    public void init(int boardWidth, int boardHeight)
    {
        startPos = new Vector3(boardWidth / 2f, boardHeight / 2f, -10f);
        maxZoom = boardWidth;
        boardSize = new Vector2(boardWidth, boardHeight);
        Camera.main.orthographicSize = maxZoom;
        transform.position = startPos;
        CalCameraMovementClamp();
    }

    void Update()
    {
        CameraPanning();
        CameraZoom();
    }

    private void CameraPanning()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var cameraNewPos = transform.position + direction;
            cameraNewPos.x = Mathf.Clamp(cameraNewPos.x, minCameraClamp.x, maxCameraClamp.x);
            cameraNewPos.y = Mathf.Clamp(cameraNewPos.y, minCameraClamp.y, maxCameraClamp.y);
            transform.position = cameraNewPos;

        }

        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }

            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                var cameraNewPos = Camera.main.transform.position + direction;
                cameraNewPos.x = Mathf.Clamp(cameraNewPos.x, minCameraClamp.x, maxCameraClamp.x);
                cameraNewPos.y = Mathf.Clamp(cameraNewPos.y, minCameraClamp.y, maxCameraClamp.y);
                Camera.main.transform.position = cameraNewPos;
            }
        }
        //Vector3 pos = transform.position;
        //pos.x = Mathf.Clamp(pos.x, minX, maxX);
        //pos.y = Mathf.Clamp(pos.y, minY, maxY);
        //transform.position = pos;
    }

    private void CameraZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Camera.main.orthographicSize -= difference * scrollSpeed * Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Camera.main.orthographicSize -= scroll * scrollSpeed * Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }
        CalCameraMovementClamp();
    }

    private void CalCameraMovementClamp()
    {
        var minClampX = (boardSize.x / 2) - (maxZoom - Camera.main.orthographicSize) / 2;
        var maxClampX = (boardSize.x / 2) + (maxZoom - Camera.main.orthographicSize) / 2;

        var clampY1 = (maxZoom - Camera.main.orthographicSize);
        var clampY2 = boardSize.y - (maxZoom - Camera.main.orthographicSize);

        minCameraClamp = new Vector2(minClampX, clampY1 < clampY2 ? clampY1 :clampY2);
        maxCameraClamp = new Vector2(maxClampX, clampY1 > clampY2 ? clampY1 :clampY2);
    }
}
