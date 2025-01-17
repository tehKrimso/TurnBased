using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
	public static LevelGrid Instance { get; private set;}
	
	public const float FLOOR_HEIGHT = 3f;
	
	public event EventHandler OnAnyUnitMovedGridPosition;
	
	[SerializeField]
	private Transform gridDebugObjectPrefab;
	
	[SerializeField]
	private int width = 10;
	[SerializeField]
	private int height = 10;
	[SerializeField]
	private float cellSize = 2f;
	[SerializeField]
	private int floorAmount
	;
	
	private List<GridSystem<GridObject>> gridSystemList;
	private void Awake()
	{
		if(Instance != null)
		{
			Debug.LogError("There's more than one LevelGrid! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		
		Instance = this;
		
		gridSystemList = new List<GridSystem<GridObject>>();
		
		for(int floor = 0; floor < floorAmount; floor++)
		{
			GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(
				width,height, cellSize, floor, FLOOR_HEIGHT, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
			gridSystemList.Add(gridSystem);
		}
		
		
		
		
		//gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
	}
	
	private void Start() 
	{
		Pathfinding.Instance.Setup(width,height,cellSize);
	}
	
	private GridSystem<GridObject> GetGridSystem(int floor)
	{
		return gridSystemList[floor];
	}
	
	public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
	{
		GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).AddUnit(unit);
		
		//unit.Move(gridSystem.GetWorldPosition(gridPosition));
	}
	
	public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
	{
		return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetUnitList();
	}
	
	public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
	{
		GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).RemoveUnit(unit);
	}
	
	public void UnitMovedGridPosition(Unit unit, GridPosition from, GridPosition to)
	{
		RemoveUnitAtGridPosition(from, unit);
		AddUnitAtGridPosition(to, unit);
		
		OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
	}
	
	public int GetFloor(Vector3 worldPosition)
	{
		return Mathf.RoundToInt(worldPosition.y/FLOOR_HEIGHT);
	}
	
	public GridPosition GetGridPosition(Vector3 worldPosition)
	{
		return GetGridSystem(GetFloor(worldPosition)).GetGridPosition(worldPosition);
	}
	
	public bool IsValidGridPosition(GridPosition gridPosition)
	{
		return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
	}
	
	public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
		return gridObject.HasAnyUnity();
	}
	
	public Unit GetUnitAtGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
		return gridObject.GetUnit();
	}
	
	public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);
	
	public int GetWidth() => GetGridSystem(0).GetWidth();
	public int GetHeight() => GetGridSystem(0).GetHeight();
	
	public int GetFloorAmount() => floorAmount;
	
	public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
		return gridObject.GetInteractable();
	}
	
	public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
	{
		GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
		gridObject.SetInteractable(interactable);
	}
}
