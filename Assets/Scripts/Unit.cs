using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Unit : MonoBehaviour
{
	[SerializeField]
	private bool isEnemy;
	private const int ACTION_POINTS_MAX = 2;
	
	public static event EventHandler OnAnyActionPointsChanged;
	
	private GridPosition gridPosition;
	private HealthSystem healthSystem;
	private MoveAction moveAction;
	private SpinAction spinAction;
	private BaseAction[] baseActionArray;
	
	
	private int actionPoints = ACTION_POINTS_MAX;
	
	
	private void Awake()
	{
		moveAction = GetComponent<MoveAction>();
		spinAction = GetComponent<SpinAction>();
		baseActionArray = GetComponents<BaseAction>();
		
		healthSystem = GetComponent<HealthSystem>();
	}
	
	private void Start()
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
		TurnSystem.Instance.OnTurnChganged += TurnSystem_OnTurnChanged;
		
		healthSystem.OnDead += HealthSystem_OnDead;
	}

	

	private void Update()
	{
		
		
		GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		if(newGridPosition != gridPosition)
		{
			GridPosition oldGridPosition = gridPosition;
			gridPosition = newGridPosition;
			LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
		}
	}
	
	public MoveAction GetMoveAction() => moveAction;
	public SpinAction GetSpinAction() => spinAction;
	
	public GridPosition GetGridPosition() => gridPosition;
	
	public BaseAction[] GetBaseActionArray() => baseActionArray;
	
	public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
	{
		if(CanSpendActionPointsToTakeAction(baseAction))
		{
			SpendActionPoints(baseAction.GetActionPointsCost());
			return true;
		}
		
		return false;
	}
	
	public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
	{
		return actionPoints >= baseAction.GetActionPointsCost();
	}
	
	private void SpendActionPoints(int amount)
	{
		actionPoints -= amount;
		
		OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
	}
	
	public int GetActionPoints() => actionPoints;
	
	private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
	{
		if((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
		   (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
		 {
		 	actionPoints = ACTION_POINTS_MAX;
		
			OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
		 }
	}
	
	private void HealthSystem_OnDead(object sender, EventArgs e)
	{
		LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition,this);
		Destroy(gameObject);
	}
	
	public bool IsEnemy() => isEnemy;

	public void Damage(int damageAmount)
	{
		Debug.Log(transform + " damage: " + damageAmount);
		healthSystem.TakeDamage(damageAmount);
	}

	public Vector3 GetWorldPosition()
	{
		return transform.position;
	}
}
