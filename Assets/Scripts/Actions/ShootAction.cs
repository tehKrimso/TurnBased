using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
	public event EventHandler<OnShootEventArgs> OnShoot;
	
	public class OnShootEventArgs : EventArgs
	{
		public Unit targetUnit;
		public Unit shootingUnit;
	}
	
	private enum State
	{
		Aiming,
		Shooting,
		Cooloff,
	}
	private State state;
	
	[SerializeField]
	private int shootDamage = 40;
	
	[SerializeField]
	private int maxShootDistance = 7;
	[SerializeField]
	private float rotationToTargetSpeed = 15f;
	[SerializeField]
	private LayerMask obstaclesLayerMask;
	private float stateTimer;
	private Unit targetUnit;
	private bool canShootBullet;
	
	
	private void Update()
	{
		if(!isActive)
		{
			return;
		}
		
		stateTimer -= Time.deltaTime;
		
		switch(state)
		{
			case State.Aiming:
				Vector3 aimDir = (targetUnit.GetWorldPosition() - transform.position).normalized;
				transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotationToTargetSpeed);
				break;
			case State.Shooting:
				if(canShootBullet)
				{
					Shoot();
					canShootBullet = false;
				}
				break;
			case State.Cooloff:
				break;
		}
		
		if(stateTimer <= 0f)
		{
			NextState();
		}
	}

	private void Shoot()
	{
		OnShoot?.Invoke(this, new OnShootEventArgs 
		{
			targetUnit = targetUnit,
			shootingUnit = unit
		});
		targetUnit.Damage(shootDamage);
	}

	private void NextState()
	{
		switch(state)
		{
			case State.Aiming:
			state = State.Shooting;
			float shootingStateTime = 0.1f;
			stateTimer = shootingStateTime;
			break;
		case State.Shooting:
			state = State.Cooloff;
			float cooloffStateTime = 0.5f;
			stateTimer = cooloffStateTime;
			break;
		case State.Cooloff:
			ActionComplete();
			break;
		}	
	}

	public override string GetActionName()
	{
		return "Shoot";
	}
	
	public override List<GridPosition> GetValidActionGridPositionList()
	{
		GridPosition unitGridPosition = unit.GetGridPosition();
		return GetValidActionGridPositionList(unitGridPosition);
	}

	public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();
		
		for( int x = -maxShootDistance; x <= maxShootDistance; x++)
		{
			for( int z = -maxShootDistance; z <= maxShootDistance; z++)
			{
				GridPosition offsetGridPosition = new GridPosition(x,z);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
				
				if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))//pos not inside grid
				{
					continue;
				}
				
				int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
				if(testDistance > maxShootDistance)
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
				
				float unitShoulderHeight = 1.7f;
				Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
				Vector3 shootDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
				
				if(Physics.Raycast(
					unitWorldPosition + Vector3.up * unitShoulderHeight,
					shootDirection,
					Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
					obstaclesLayerMask ))
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

		state = State.Aiming;
		float aimingStateTime = 1f;
		stateTimer = aimingStateTime;
		
		canShootBullet = true;
		
		ActionStart(onActionComplete);
	}
	
	public Unit GetTargetUnit() => targetUnit;
	
	public int GetMaxShootDistance() => maxShootDistance;
	
	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
		
		return new EnemyAIAction 
		{
			gridPosition = gridPosition,
			actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized() * 100f)),
		};
	}
	
	public int GetTargetCountAtPosition(GridPosition gridPosition)
	{
		return GetValidActionGridPositionList(gridPosition).Count;
	}
}
