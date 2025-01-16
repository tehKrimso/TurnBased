using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
	[SerializeField]
	private Material Green;
	[SerializeField]
	private Material Red;
	[SerializeField]
	private MeshRenderer meshRenderer;
	private bool isGreen;
	private bool isActive;
	private float timer;
	private Action OnInteractComplete;
	private GridPosition gridPosition;
	
	private void Start()
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
		SetColorGreen();
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
	
	private void SetColorGreen()
	{
		meshRenderer.material = Green;
		isGreen = true;
	}
	
	private void SetColorRed()
	{
		meshRenderer.material = Red;
		isGreen = false;
	}

	public void Interact(Action onInteractionComplete)
	{
		OnInteractComplete = onInteractionComplete;
		isActive = true;
		timer = 0.5f;
		
		if(isGreen)
		{
			SetColorRed();
		}
		else
		
		{
			SetColorGreen();
		}
	}
}
