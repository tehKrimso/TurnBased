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
				
				IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
				if(interactable == null)
				{
					//no interactable
					continue;
				}
				
				validGridPositionList.Add(testGridPosition);
				
			}
		}
		
		return validGridPositionList;
	}

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		
		IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
		interactable.Interact(OnInteractComplete);
		ActionStart(onActionComplete);
	}
	
	private void OnInteractComplete()
	{
		ActionComplete();
	}
}
