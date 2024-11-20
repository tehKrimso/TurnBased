using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
	public delegate void SpinCompleteDelegate();
	
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
			isActive = false;
			onActionComplete();
		}

		float spinAddAmount = targetSpin * Time.deltaTime;
		transform.eulerAngles += new Vector3(0,spinAddAmount, 0);
		totalSpinAmount += spinAddAmount;
	}
	
	public override void TakeAction(GridPosition grdidPosition, Action onActionComplete)
	{
		this.onActionComplete = onActionComplete;
		isActive = true;

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
        return 2;
    }
}
