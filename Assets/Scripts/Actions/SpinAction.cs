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
	
	public void Spin(Action onActionComplete)
	{
		this.onActionComplete = onActionComplete;
		isActive = true;

	}
}
