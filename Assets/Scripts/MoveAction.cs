using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour
{
	[SerializeField]
	private int maxMoveDistance = 4;
	
	[SerializeField]
	private float moveSpeed = 4f;
	
	[SerializeField]
	private float rotationSpeed = 10f;
	
	[SerializeField]
	private float distanceThreshold = 0.1f;
	
	[SerializeField]
	private Animator unitAnimator;
	private Vector3 targetPosition;
	private Unit unit;
	
	private void Awake()
	{
		targetPosition = transform.position;
		unit = GetComponent<Unit>();
	}

	private void Update()
	{
		if(Vector3.Distance(transform.position, targetPosition) > distanceThreshold)
		{
			Vector3 moveDirection = (targetPosition - transform.position).normalized;
			transform.position += moveDirection * moveSpeed * Time.deltaTime;
			
			
			transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
			unitAnimator.SetBool("IsWalking", true);
		}
		else
		{
			unitAnimator.SetBool("IsWalking", false);
		}
	
	}
	
	public void Move(GridPosition gridPosition)
	{
		this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
	}
	
	public bool IsValidActionGridPosition(GridPosition gridPosition)
	{
		List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
		return validGridPositionList.Contains(gridPosition);
	}
	
	public List<GridPosition> GetValidActionGridPositionList()
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();
		
		GridPosition unitGridPosition = unit.GetGridPosition();
		
		for( int x = -maxMoveDistance; x <= maxMoveDistance; x++)
		{
			for( int z = -maxMoveDistance; z <= maxMoveDistance; z++)
			{
				GridPosition offsetGridPosition = new GridPosition(x,z);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
				
				if(LevelGrid.Instance.IsValidGridPosition(testGridPosition) && //pos inside grid
				   testGridPosition != unitGridPosition && //position not same as unit pos
				   !LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) //pos not occupied
				{
					validGridPositionList.Add(testGridPosition);
					//Debug.Log(testGridPosition);
				}
				
			}
		}
		
		return validGridPositionList;
	}
}
