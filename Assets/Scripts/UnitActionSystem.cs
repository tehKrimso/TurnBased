using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
	public static UnitActionSystem Instance { get; private set;}
	
	public event EventHandler OnSelectedUnitChange;
	
	[SerializeField]
	private Unit _selectedUnit;
	[SerializeField]
	private LayerMask _unitLayerMask;
	
	private BaseAction _selectedAction;
	private bool isBusy;
	
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
	
	private void Start() 
	{
		SetSelectedUnit(_selectedUnit);	
	}
	
	private void Update()
	{
		if(isBusy)
		{
			return;
		}
		
		if(EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		
		if(TryHandleUnitSelection())
		{
			return;
		}
		
		HandleSelectedAction();
	}
	
	private void HandleSelectedAction()
	{
		
		
		if(Input.GetMouseButtonDown(0))
		{
			GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
			if(_selectedAction.IsValidActionGridPosition(mouseGridPosition))
			{
				SetBusy();
				_selectedAction.TakeAction(mouseGridPosition, ClearBusy);
			}
		}
	}
	
	private void SetBusy()
	{
		isBusy = true;
	}
	
	private void ClearBusy()
	{
		isBusy = false;
	}
	
	private bool TryHandleUnitSelection()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _unitLayerMask))
			{
				if(raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
				{
					if(unit == _selectedUnit)
					{
						return false;
					}
					
					SetSelectedUnit(unit);
					return true;
				}
			}	
		}
		
		return false;
	}
	
	private void SetSelectedUnit(Unit unit)
	{
		_selectedUnit = unit;
		SetSelectedAction(unit.GetMoveAction());
		OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
	}
	
	public void SetSelectedAction(BaseAction baseAction)
	{
		_selectedAction = baseAction;
	}
	
	public Unit GetSelectedUnit()
	{
		return _selectedUnit;
	}
	
	public BaseAction GetSelectedAction()
	{
		return _selectedAction;
	}
}
