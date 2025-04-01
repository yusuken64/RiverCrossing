using System;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    public LayerMask GridLayer;
    public Action OnHold;
    public Action<Cell> OnReleased;
    public Action OnClcked;

    public bool IsDraggable;

    public float HoldThresholdSeconds;
    public float HoldTimeSeconds;

    void OnMouseDown()
    {
        if (!IsDraggable)
        {
            return;
        }
        offset = transform.position - GetMouseWorldPosition();
        var hit = GetCellUnderneath();

        var cell = hit?.GetComponent<Cell>();
        if (cell != null)
        {
          isDragging = true;
          HoldTimeSeconds = 0;
          OnHold?.Invoke();
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    void OnMouseUp()
    {
        if (!IsDraggable)
        {
            return;
        }

        isDragging = false;

        if (HoldTimeSeconds < HoldThresholdSeconds)
        {
            OnClcked?.Invoke();
        }
        else
        {
            var collider = GetCellUnderneath();
            Cell cell = null;
            if (collider != null)
            {
                cell = collider.GetComponent<Cell>();
            }
            OnReleased?.Invoke(cell);
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            HoldTimeSeconds += Time.deltaTime;
        }
    }

    private Collider2D GetCellUnderneath()
    {
        Collider2D collider = Physics2D.OverlapPoint(this.transform.position, GridLayer);
        return collider;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z; // Maintain depth
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
