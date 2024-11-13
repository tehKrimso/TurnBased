using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Unit : MonoBehaviour
{
	[SerializeField]
	private float moveSpeed = 4f;
	
	[SerializeField]
	private float rotationSpeed = 10f;
	
	[SerializeField]
	private float distanceThreshold = 0.1f;
	
	[SerializeField]
	private Animator unitAnimator;
	private Vector3 targetPosition;
	
	private GridPosition gridPosition;
	
	
	private void Awake()
	{
		targetPosition = transform.position;
	}
	
	private void Start()
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
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
		
		GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		if(newGridPosition != gridPosition)
		{
			
			LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
			gridPosition = newGridPosition;
		}
	}
	
	public void Move(Vector3 targetPosition)
	{
		this.targetPosition = targetPosition;
	}
}
