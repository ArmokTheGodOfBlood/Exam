using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildController : MonoBehaviour
{
    public static BuildController instance;

    InputActions input;

    [SerializeField] Grid placementGrid;
    [SerializeField] GameObject placementParent;
    [SerializeField] GameObject buildingPrefab;
    Building_SO building_SO;

    Dictionary<Vector2Int, Building> occupiedCells = new Dictionary<Vector2Int, Building>();
    public event Action<Dictionary<Vector2Int, Building>> OnBuildingDataChanged;

    private void Awake()
    {
        instance = this;

        input = new InputActions();
        input.Enable();

        enabled = false;
    }

    // В апдейте - логика движения "призрака" здания
    private void Update()
    {
        Vector3 worldPos = GetMouseWorldPosition();
        Vector3Int cellPos = placementGrid.WorldToCell(worldPos);
        Vector3 snapPos = placementGrid.CellToWorld(cellPos);

        ghostBuilding.transform.position = snapPos;
        if (CanPlaceBuilding(new Vector2Int(cellPos.x, cellPos.y), building_SO.size))
            ghostRenderer.color = new Color(0f, 1f, 0f, 0.5f);
        else
            ghostRenderer.color = new Color(1f, 0f, 0f, 0.5f);
    }

    private GameObject ghostBuilding;
    private SpriteRenderer ghostRenderer;
    public void SetBuilding(Building_SO newBuildingSO)
    {
        building_SO = newBuildingSO;
    }
    public void StartBuilding()
    {
        if (ghostBuilding != null)
            Destroy(ghostBuilding);

        ghostBuilding = new GameObject("GhostBuilding");
        ghostRenderer = ghostBuilding.AddComponent<SpriteRenderer>();
        ghostRenderer.sprite = building_SO.sprite;
        ghostRenderer.color = new Color(0f, 1f, 0f, 0.5f);
        ghostRenderer.sortingLayerName = "UI";

        enabled = true;
        input.BuildingMapping.LeftClick.performed += PlaceBuilding;
    }
    private void PlaceBuilding(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 worldPos = GetMouseWorldPosition();
        Vector3Int cellPos = placementGrid.WorldToCell(worldPos);
        Vector3 snapPos = placementGrid.CellToWorld(cellPos);

        if (!CanPlaceBuilding(new Vector2Int(cellPos.x, cellPos.y), building_SO.size))
            return;

        PlaceBuilding(snapPos, building_SO);

        Destroy(ghostBuilding);
        enabled = false;
        input.BuildingMapping.LeftClick.performed -= PlaceBuilding;

        OnBuildingDataChanged?.Invoke(occupiedCells);
    }

    public void PlaceBuilding(Vector3 snapPos, Building_SO _building_SO)
    {
        GameObject buildingInstance = Instantiate(
            buildingPrefab,
            snapPos,
            Quaternion.identity);

        buildingInstance.transform.parent = placementParent.transform;

        Building buildingComponent = buildingInstance.GetComponent<Building>();
        buildingComponent.Initiate(_building_SO);
        // Гарантирует, что дальние здания будут отрисованы под ближними.
        buildingInstance.GetComponent<SpriteRenderer>().sortingOrder = -(int)(snapPos.y / placementGrid.cellSize.y);



        // Записать занятые зданием клетки
        for (int x = 0; x < _building_SO.size.x; x++)
        {
            for (int y = 0; y < _building_SO.size.y; y++)
            {
                Vector2Int occupiedCell = new Vector2Int((int)snapPos.x + x, (int)snapPos.y + y);
                occupiedCells.Add(occupiedCell, buildingComponent);
            }
        }
    }

    public void StartRemoval()
    {
        if (ghostBuilding != null)
            Destroy(ghostBuilding);

        input.BuildingMapping.LeftClick.performed += RemoveBuilding;
    }
    public void RemoveBuilding(InputAction.CallbackContext ctx)
    {
        Vector3 worldPos = GetMouseWorldPosition();
        Vector2Int cellPos = (Vector2Int)placementGrid.WorldToCell(worldPos);

        if (!IsCellOccupied(new Vector2Int(cellPos.x, cellPos.y)))
            return;

        Building target = occupiedCells[cellPos];

        List<Vector2Int> statedCells = new List<Vector2Int>();
        statedCells = occupiedCells
            .Where(entry => entry.Value == target)
            .Select(entry => entry.Key)
            .ToList();

        foreach (var cell in statedCells)
            occupiedCells.Remove(cell);

        Destroy(target.gameObject);
        input.BuildingMapping.LeftClick.performed -= RemoveBuilding;

        OnBuildingDataChanged?.Invoke(occupiedCells);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
    }

    bool CanPlaceBuilding(Vector2Int startCell, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (occupiedCells.Keys.Contains(startCell + new Vector2Int(x, y)))
                    return false;
            }
        }
        return true;
    }
    bool IsCellOccupied(Vector2Int cellPos)
    {
        return occupiedCells.Keys.Contains(cellPos);
    }

    public Dictionary<Vector2Int, Building> GetBuildings() => occupiedCells;
}
