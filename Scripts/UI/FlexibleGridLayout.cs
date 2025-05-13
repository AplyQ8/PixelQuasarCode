using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private Vector2 cellSize;
    [SerializeField] private Vector2 spacing;
    [SerializeField] private FitType fitType;
    [SerializeField] private bool fitX;
    [SerializeField] private bool fitY;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (fitType == FitType.Height || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            fitX = true;
            fitY = true;
            float sqrRt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
        }

        if (fitType is FitType.Width or FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);;
        }

        if (fitType is FitType.Height or FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }
        
        
        
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = 
            parentWidth / (float)columns 
            - ((spacing.x / (float)columns) * 2)
            - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = 
            parentHeight / (float)rows 
            - ((spacing.y / (float)rows) * 2)
            - (padding.top / (float)rows) - (padding.bottom / (float)rows);

        cellSize.x = fitX? cellWidth : cellSize.x;
        cellSize.y = fitY? cellHeight : cellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];

            var xPosition = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPosition = (cellSize.y * rowCount) + (spacing.y + rowCount) + padding.top;
            
            SetChildAlongAxis(item, 0, xPosition, cellSize.x);
            SetChildAlongAxis(item, 0, yPosition, cellSize.y);
        }
    }
    public override void CalculateLayoutInputVertical()
    {
        
    }

    public override void SetLayoutHorizontal()
    {
        
    }

    public override void SetLayoutVertical()
    {
        
    }
}
