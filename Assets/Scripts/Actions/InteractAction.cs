using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
	[SerializeField]
	private int maxInteractionDistance = 1;
	
	
	private void Update()
	{
		if(!isActive)
		{
			return;
		}
	}
	
	public override string GetActionName()
	{
		return "Interact";
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		return new EnemyAIAction
		{
			gridPosition = gridPosition,
			actionValue = 0
		};
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();
		
		GridPosition unitGridPosition = unit.GetGridPosition();
		
		for( int x = -maxInteractionDistance; x <= maxInteractionDistance; x++)
		{
			for( int z = -maxInteractionDistance; z <= maxInteractionDistance; z++)
			{
				GridPosition offsetGridPosition = new GridPosition(x,z);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
				
				if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))//pos not inside grid
				{
					continue;
				}
				
				Door door = LevelGrid.Instance.GetDoorAtGridPosition(testGridPosition);
				if(door == null)
				{
					//noo door
					continue;
				}
				
				validGridPositionList.Add(testGridPosition);
				
			}
		}
		
		return validGridPositionList;
	}

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		
		Door door = LevelGrid.Instance.GetDoorAtGridPosition(gridPosition);
		door.Interact(OnInteractComplete);
		Debug.Log("InteractAction");
		ActionStart(onActionComplete);
	}
	
	private void OnInteractComplete()
	{
		ActionComplete();
	}
}
