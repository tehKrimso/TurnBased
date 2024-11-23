using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HealthSystem : MonoBehaviour
{
	public event EventHandler OnDead;
	
	[SerializeField]
	private int health = 100;
	
	public void TakeDamage(int damageAmount)
	{
		health -= damageAmount;
		
		if(health<0)
		{
			health = 0;
		}
		
		if(health==0)
		{
			Die();
		}
		
		Debug.Log(health);
	}
	
	
	private void Die()
	{
		OnDead?.Invoke(this, EventArgs.Empty);
	}
}
