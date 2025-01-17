using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveAction : BaseAction
{
	public event EventHandler OnStartMoving;
	public event EventHandler OnStopMoving;
	public event EventHandler<OnChangeFloorsStartEventArgs> OnChangeFloorsStart;
	
	public class OnChangeFloorsStartEventArgs: EventArgs
	{
		public GridPosition unitGridPosition;
		public GridPosition targetGridPosition;
	}
	
	[SerializeField]
	private int maxMoveDistance = 4;
	
	[SerializeField]
	private float moveSpeed = 4f;
	
	[SerializeField]
	private float rotationSpeed = 10f;
	
	[SerializeField]
	private float distanceThreshold = 0.1f;
	
	private List<Vector3> positionList;
	private int currentPositionIndex;
	
	private bool isChangingFloors;
	private float differentFloorsTeleportTimer;
	private float differentFloorsTeleportTimerMax = 0.5f;


	private void Update()
	{
		if(!isActive)
			return;
			
		
		Vector3 targetPosition = positionList[currentPositionIndex];
		if(isChangingFloors)
		{
			Vector3 targetSameFloorPosition = targetPosition;
			targetSameFloorPosition.y = transform.position.y;
			
			Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized;
		
			transform.forward = Vector3.Slerp(transform.forward, rotateDirection, Time.deltaTime * rotationSpeed);
			
			differentFloorsTeleportTimer -= Time.deltaTime;
			if(differentFloorsTeleportTimer < 0f)
			{
				isChangingFloors = false;
				transform.position = targetPosition;
			}
		}
		else
		{
			Vector3 moveDirection = (targetPosition - transform.position).normalized;
		
			transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
			transform.position += moveDirection * moveSpeed * Time.deltaTime;
		}
		
		
		
		if(Vector3.Distance(transform.position, targetPosition) < distanceThreshold)
		{
			currentPositionIndex++;
			if(currentPositionIndex >= positionList.Count)
			{
				OnStopMoving?.Invoke(this, EventArgs.Empty);
				ActionComplete();
			}
			else
			{
				targetPosition = positionList[currentPositionIndex];
				GridPosition targetGridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
				GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
				
				if(targetGridPosition.floor != unitGridPosition.floor)
				{
					isChangingFloors = true;
					differentFloorsTeleportTimer = differentFloorsTeleportTimerMax;
					
					OnChangeFloorsStart?.Invoke(this, new OnChangeFloorsStartEventArgs
					{
						unitGridPosition = unitGridPosition,
						targetGridPosition = targetGridPosition
					});
				}
			}
		}
	}
	
	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		currentPositionIndex = 0;
		positionList = Pathfinding.Instance
		.FindPath(unit.GetGridPosition(),gridPosition, out int pathLength)
		.Select(gp => LevelGrid.Instance.GetWorldPosition(gp))
		.ToList();
		OnStartMoving?.Invoke(this, EventArgs.Empty);
		ActionStart(onActionComplete);
	}
	
	public override List<GridPosition> GetValidActionGridPositionList()
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();
		
		GridPosition unitGridPosition = unit.GetGridPosition();
		
		int pathfindingDistanceMultiplier = 10;
		
		for( int x = -maxMoveDistance; x <= maxMoveDistance; x++)
		{
			for( int z = -maxMoveDistance; z <= maxMoveDistance; z++)
			{
				for(int floor = -maxMoveDistance; floor <= maxMoveDistance; floor ++)
				{
					GridPosition offsetGridPosition = new GridPosition(x,z, floor);
					GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
				
					if(LevelGrid.Instance.IsValidGridPosition(testGridPosition) && //pos inside grid
				   		testGridPosition != unitGridPosition && //position not same as unit pos
				   		!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition) //pos not occupied
				   		&& Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) < maxMoveDistance * pathfindingDistanceMultiplier //pos within move distance
				   		&& Pathfinding.Instance.IsWalkableGridPosition(testGridPosition) //pos is walkable
				   		&& Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition) //pos is reachable
				   		)
						{
							validGridPositionList.Add(testGridPosition);
						}
				}
			}
		}
		
		return validGridPositionList;
	}

	public override string GetActionName()
	{
		return "Move";
	}
	
	
	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		ShootAction shootAction = unit.GetAction<ShootAction>();
		int targetCountAtPosition = shootAction.GetTargetCountAtPosition(gridPosition);
		
		if (targetCountAtPosition == 0)
	{
		return new EnemyAIAction
		{
			gridPosition = gridPosition,
			actionValue = UnityEngine.Random.Range(1, 51)
		};
	}
	else
	{
		return new EnemyAIAction
		{
			gridPosition = gridPosition,
			actionValue = 50 + targetCountAtPosition * 10
		};
	}
	}
}
