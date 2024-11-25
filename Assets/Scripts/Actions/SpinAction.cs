using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
	[SerializeField]
	private float targetSpin = 360f;
	
	private float totalSpinAmount;
	
	private void Update()
	{
		if(!isActive)
		{
			return;
		}
		
		if(totalSpinAmount>= targetSpin)
		{
			totalSpinAmount = 0f;
			ActionComplete();
		}

		float spinAddAmount = targetSpin * Time.deltaTime;
		transform.eulerAngles += new Vector3(0,spinAddAmount, 0);
		totalSpinAmount += spinAddAmount;
	}
	
	public override void TakeAction(GridPosition grdidPosition, Action onActionComplete)
	{
		ActionStart(onActionComplete);
	}
	
	public override string GetActionName()
	{
		return "Spin";
	}
	
	public override List<GridPosition> GetValidActionGridPositionList()
	{
		GridPosition unitGridPosition = unit.GetGridPosition();
		
		return new List<GridPosition>
		{
			unitGridPosition
		};
	}

	public override int GetActionPointsCost()
	{
		return 1;
	}
	
	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		return new EnemyAIAction 
		{
			gridPosition = gridPosition,
			actionValue = 0,
		};
	}
}
