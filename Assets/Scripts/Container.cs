using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public int Width;
    public int Height;

    public float xOffset;
    public float yOffset;

    public Transform CellContainer;
    public Cell CellPrefab;
    public List<Cell> Cells;

    public Transform StartPositionObject;

    [ContextMenu("Setup Cells")]
    public void SetupCells()
    {
        ClearChildren();

        Cells.Clear();
        var startPosition = StartPositionObject.position;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                var newCell = Instantiate(CellPrefab, CellContainer);
                newCell.transform.position = 
                    new Vector3(
                        startPosition.x + (i * xOffset),
                        startPosition.y + (j * yOffset),
                        startPosition.z);
                Cells.Add(newCell);
            }
        }
    }

    [ContextMenu("Clear Cells")]
    public void ClearChildren()
    {
#if UNITY_EDITOR
        foreach (Transform child in CellContainer)
        {
            DestroyImmediate(child.gameObject);
        }

        Cell[] cells = CellContainer.GetComponentsInChildren<Cell>(true);
        foreach (Cell cell in cells)
        {
            DestroyImmediate(cell.gameObject);
        }

#else
        foreach (Transform child in CellContainer)
        {
            Destroy(child.gameObject);
        }
#endif
    }
}
