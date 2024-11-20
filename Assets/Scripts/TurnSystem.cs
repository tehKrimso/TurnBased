using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnSystem : MonoBehaviour
{
	public static TurnSystem Instance { get; private set;}
	
	public event EventHandler OnTurnChganged;
	
	private int turnNumber = 1;
	
	private void Awake()
	{
		if(Instance != null)
		{
			Debug.LogError("There's more than one TurnSystem! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		
		Instance = this;
	}
	
	public void NextTurn()
	{
		turnNumber++;
		OnTurnChganged?.Invoke(this, EventArgs.Empty);
	}
	
	public int GetCurrentTurnNumber() => turnNumber;
}
