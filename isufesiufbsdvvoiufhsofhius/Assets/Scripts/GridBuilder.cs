using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridBuilder : MonoBehaviour
{
    public Vector2 spacing = new Vector2(8,8);
    public RectOffset padding = new RectOffset(8,8,8,8);

    // Applies cellSize (computed) to GridLayoutGroup on target container
    public void ApplyGrid(RectTransform container, int rows, int cols)
    {
        GridLayoutGroup grid = container.GetComponent<GridLayoutGroup>();
        if (grid == null) grid = container.gameObject.AddComponent<GridLayoutGroup>();
        grid.spacing = spacing;
        grid.padding = padding;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = cols;

        // compute cell size to fit container
        float width = container.rect.width - padding.left - padding.right - spacing.x * (cols - 1);
        float height = container.rect.height - padding.top - padding.bottom - spacing.y * (rows - 1);

        float cellW = width / cols;
        float cellH = height / rows;
        float cellSize = Mathf.Floor(Mathf.Min(cellW, cellH));

        grid.cellSize = new Vector2(cellSize, cellSize);
    }
}
