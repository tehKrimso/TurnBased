using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
	public static LevelGrid Instance { get; private set;}
	
	public event EventHandler OnAnyUnitMovedGridPosition;
	
	[SerializeField]
	private Transform gridDebugObjectPrefab;
	
	[SerializeField]
	private int width = 10;
	[SerializeField]
	private int height = 10;
	[SerializeField]
	private float cellSize = 2f;
	
	private GridSystem<GridObject> gridSystem;
	private void Awake()
	{
		if(Instance != null)
		{
			Debug.LogError("There's more than one LevelGrid! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		
		Instance = this;
		
		gridSystem = new GridSystem<GridObject>(width,height, cellSize, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
		
		
		//gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
	}
	
	private void Start() 
	{
		Pathfinding.Instance.Setup(width,height,cellSize);
	}
	
	public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
	{
		gridSystem.GetGridObject(gridPosition).AddUnit(unit);
		
		//unit.Move(gridSystem.GetWorldPosition(gridPosition));
	}
	
	public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
	{
		return gridSystem.GetGridObject(gridPosition).GetUnitList();
	}
	
	public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
	{
		gridSystem.GetGridObject(gridPosition).RemoveUnit(unit);
	}
	
	public void UnitMovedGridPosition(Unit unit, GridPosition from, GridPosition to)
	{
		RemoveUnitAtGridPosition(from, unit);
		AddUnitAtGridPosition(to, unit);
		
		OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
	}
	
	public GridPosition GetGridPosition(Vector3 worldPosition)
	{
		return gridSystem.GetGridPosition(worldPosition);
	}
	
	public bool IsValidGridPosition(GridPosition gridPosition)
	{
		return gridSystem.IsValidGridPosition(gridPosition);
	}
	
	public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.HasAnyUnity();
	}
	
	public Unit GetUnitAtGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.GetUnit();
	}
	
	public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
	
	public int GetWidth() => gridSystem.GetWidth();
	public int GetHeight() => gridSystem.GetHeight();
	
	public Door GetDoorAtGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.GetDoor();
	}
	
	public void SetDoorAtGridPosition(GridPosition gridPosition, Door door)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		gridObject.SetDoor(door);
	}
}
