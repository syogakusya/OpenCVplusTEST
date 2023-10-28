using UnityEngine;

public class DragObj : MonoBehaviour
{
    private Vector3 screenPoint;
    private float screenX;
    private float screenY;
    private float screenZ;
    private Vector3 currentScreenPoint;
    private Vector3 currentPosition;

    void OnMouseDrag()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        screenX = Input.mousePosition.x;
        screenY = Input.mousePosition.y;
        screenZ = screenPoint.z;

        currentScreenPoint = new Vector3(screenX, screenY, screenZ);
        currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint);
        transform.position = currentPosition;
    }
}