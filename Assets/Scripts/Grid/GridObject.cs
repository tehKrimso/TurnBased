using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GridObject
{
	private GridSystemHex<GridObject> gridSystem;
	private GridPosition gridPosition;
	private List<Unit> unitList;
	private IInteractable interactable;

	public GridObject(GridSystemHex<GridObject> gridSystem, GridPosition gridPosition)
	{
		this.gridSystem = gridSystem;
		this.gridPosition = gridPosition;
		unitList = new List<Unit>();
	}
	
	public void AddUnit(Unit unit) => unitList.Add(unit);
	
	public List<Unit> GetUnitList() => unitList;
	
	public void RemoveUnit(Unit unit) => unitList.Remove(unit);
	
	public bool HasAnyUnity() => unitList.Count > 0;

	public override string ToString()
	{
		string unitString = "";
		foreach(Unit unit in unitList)
		{
			unitString+=unit + "\n";
		}
		
		return $"{gridPosition.ToString()}\n{unitString}";
	}

	public Unit GetUnit()
	{
		if(HasAnyUnity())
		{
			return unitList[0];
		}
		
		return null;
	}
	
	public IInteractable GetInteractable() => interactable;
	public void SetInteractable(IInteractable interactable) => this.interactable = interactable;
}
