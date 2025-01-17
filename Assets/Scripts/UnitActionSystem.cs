using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
	public static UnitActionSystem Instance { get; private set;}
	
	public event EventHandler OnSelectedUnitChange;
	public event EventHandler OnSelectedActionChange;
	public event EventHandler<bool> OnBusyChange;
	public event EventHandler OnActionStarted;
	
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
		//Debug.Log(LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition()));
		
		if(isBusy)
		{
			return;
		}
		
		if(!TurnSystem.Instance.IsPlayerTurn())
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
		if(InputManager.Instance.IsMouseButtonDownThisFrame())
		{
			GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPositionOnlyHitVisible());
			if(!_selectedAction.IsValidActionGridPosition(mouseGridPosition))
			{
				return;
			}
			
			if(_selectedUnit.TrySpendActionPointsToTakeAction(_selectedAction))
			{
				SetBusy();
				_selectedAction.TakeAction(mouseGridPosition, ClearBusy);
				
				OnActionStarted?.Invoke(this, EventArgs.Empty);
			}
		}
	}
	
	private void SetBusy()
	{
		isBusy = true;
		OnBusyChange?.Invoke(this, isBusy);
	}
	
	private void ClearBusy()
	{
		isBusy = false;
		OnBusyChange?.Invoke(this, isBusy);
	}
	
	private bool TryHandleUnitSelection()
	{
		if(InputManager.Instance.IsMouseButtonDownThisFrame())
		{
			Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
			if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _unitLayerMask))
			{
				if(raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
				{
					if(unit == _selectedUnit)
					{
						return false;
					}
					
					if(unit.IsEnemy())
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
		SetSelectedAction(unit.GetAction<MoveAction>());
		OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
	}
	
	public void SetSelectedAction(BaseAction baseAction)
	{
		_selectedAction = baseAction;
		OnSelectedActionChange?.Invoke(this, EventArgs.Empty);
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
