using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HealthSystem : MonoBehaviour
{
	public event EventHandler OnDead;
	public event EventHandler OnDamaged;
	
	[SerializeField]
	private int healtMax = 100;
	
	private int health;
	
	private void Awake()
	{
		health = healtMax;
	}
	
	public void TakeDamage(int damageAmount)
	{
		health -= damageAmount;
		
		if(health<0)
		{
			health = 0;
		}
		
		OnDamaged?.Invoke(this, EventArgs.Empty);
		
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
	
	public float GetHealthNormalized() => (float)health / healtMax;
}
