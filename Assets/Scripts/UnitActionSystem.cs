using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
	public static UnitActionSystem Instance { get; private set;}
	
	public event EventHandler OnSelectedUnitChange;
	
	[SerializeField]
	private Unit _selectedUnit;
	[SerializeField]
	private LayerMask _unitLayerMask;
	
	private void Awake()
	{
		if(Instance != null)
		{
			Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		
		Instance = this;
	}
	
	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			if(TryHandleUnitSelection()) return;
			
			GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
			
			if(_selectedUnit.GetMoveAction().IsValidActionGridPosition(mouseGridPosition))
			{
				_selectedUnit.GetMoveAction().Move(mouseGridPosition);
			}
		}
		
		if(Input.GetMouseButtonDown(1))
		{
			_selectedUnit.GetSpinAction().Spin();
		}
	}
	
	private bool TryHandleUnitSelection()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _unitLayerMask))
		{
			if(raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
			{
				SetSelectedUnit(unit);
				return true;
			}
		}
		
		return false;
	}
	
	private void SetSelectedUnit(Unit unit)
	{
		_selectedUnit = unit;
		OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
	}
	
	public Unit GetSelectedUnit()
	{
		return _selectedUnit;
	}
}
