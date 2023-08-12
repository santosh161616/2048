using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDirectionDetector : MonoBehaviour
{
    private Vector2 touchStartPos;
    private float swipeThreshold = 100f; // Adjust this value for sensitivity
    TileBoard boardInstance;

    private void Start()
    {
        boardInstance = GetComponent<TileBoard>();
    }

    void Update()
    {
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
                            Debug.Log("Swipe Right");
                        }
                        else if (angle >= 45f && angle < 135f)
                        {
                            boardInstance.MoveTiles(Vector2Int.up, 0, 1, 1, 1);
                            Debug.Log("Swipe Up");
                        }
                        else if (angle >= 135f || angle <= -135f)
                        {
                            Debug.Log("Swipe Left");
                        }
                        else if (angle > -135f && angle < -45f)
                        {
                            Debug.Log("Swipe Down");
                        }
                    }
                    break;
            }
        }
    }
}
