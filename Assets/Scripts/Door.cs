using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
	[SerializeField]
	private bool isOpen;
	
	private GridPosition gridPosition;
	private Animator animator;
	private Action OnInteractComplete;
	private bool isActive;
	private float timer;
	
	private void Awake()
	{
		animator = GetComponent<Animator>();
	}
	
	private void Start()
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
		
		if(isOpen)
		{
			OpenDoor();
		}
		else
		{
			CloseDoor();
		}
	}
	
	private void Update()
	{
		if(!isActive)
		{
			return;
		}
		
		timer -= Time.deltaTime;
		
		if (timer <= 0f)
		{
			isActive = false;
			OnInteractComplete();
		}
	}
	
	public void Interact(Action onInteractComplete)
	{
		OnInteractComplete = onInteractComplete;
		isActive = true;
		timer = 0.5f;
		
		if(isOpen)
		{
			CloseDoor();
		}
		else
		{
			OpenDoor();
		}
	}
	
	private void OpenDoor()
	{
		isOpen = true;
		animator.SetBool("IsOpen", true);
		Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
	}
	
	private void CloseDoor()
	{
		isOpen = false;
		animator.SetBool("IsOpen", false);
		Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
	}
}
