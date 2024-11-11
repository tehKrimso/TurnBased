using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Unit : MonoBehaviour
{
	public float moveSpeed = 4f;
	public float distanceThreshold = 0.1f;
	
	private Vector3 targetPosition;
	
	private void Update()
	{
		if(Vector3.Distance(transform.position, targetPosition) > distanceThreshold)
		{
			Vector3 moveDirection = (targetPosition - transform.position).normalized;
			transform.position += moveDirection * moveSpeed * Time.deltaTime;
		}
			
		
		if(Input.GetKeyDown(KeyCode.T))
		{
			Move(new Vector3(4,0,4));
		}
	}
	
	private void Move(Vector3 targetPosition)
	{
		this.targetPosition = targetPosition;
		
	}
}