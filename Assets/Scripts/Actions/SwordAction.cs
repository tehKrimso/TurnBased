using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
	public static event EventHandler OnAnySwordHit;
	
	[SerializeField]
	private int maxSwordDistance = 1;
	[SerializeField]
	private float rotationToTargetSpeed = 10f;
	
	public event EventHandler OnSwordActionStarted;
	public event EventHandler OnSwordActionCompleted;
	
	private enum State
	{
		SwingingSwordBeforHit,
		SwingingSwordAfterHit,
	}
	
	private State state;
	private float stateTimer;
	
	private Unit targetUnit;
	
	private void Update()
	{
		if(!isActive)
		{
			return;
		}
		
		stateTimer -= Time.deltaTime;
		
		switch(state)
		{
			case State.SwingingSwordBeforHit:
				Vector3 aimDir = (targetUnit.GetWorldPosition() - transform.position).normalized;
				transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotationToTargetSpeed);
				break;
			case State.SwingingSwordAfterHit:
				break;
		}
		
		if(stateTimer <= 0f)
		{
			NextState();
		}
	}
	
	private void NextState()
	{
		switch(state)
		{
			case State.SwingingSwordBeforHit:
				state = State.SwingingSwordAfterHit;
				float afterHitStateTime = 0.1f;
				stateTimer = afterHitStateTime;
				targetUnit.Damage(100);
				OnAnySwordHit?.Invoke(this, EventArgs.Empty);
				break;
			case State.SwingingSwordAfterHit:
				OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
				ActionComplete();
				break;
		}	
	}
	
	public override string GetActionName()
	{
		return "Sword";
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		return new EnemyAIAction
		{
			gridPosition = gridPosition,
			actionValue = 200,
		};
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();
		
		GridPosition unitGridPosition = unit.GetGridPosition();
		
		for( int x = -maxSwordDistance; x <= maxSwordDistance; x++)
		{
			for( int z = -maxSwordDistance; z <= maxSwordDistance; z++)
			{
				GridPosition offsetGridPosition = new GridPosition(x,z, 0);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
				
				if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))//pos not inside grid
				{
					continue;
				}
				
				if(!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) //pos not occupied
				{
					continue;
				}
				
				Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
				
				//no nullref check coz previous if states that position IS occupied
				if(targetUnit.IsEnemy() == unit.IsEnemy()) //units on same team
				{
					continue;
				}
				
				validGridPositionList.Add(testGridPosition);
				
			}
		}
		
		return validGridPositionList;
	}

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
		
		state = State.SwingingSwordBeforHit;
		float beforeHitStateTime = 0.7f;
		stateTimer = beforeHitStateTime;
		
		OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
		
		ActionStart(onActionComplete);
	}

	public int GetMaxSwordDistance()
	{
		return maxSwordDistance;
	}
}
