using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
	[SerializeField]
	private float moveSpeed = 15f;
	[SerializeField]
	private float reachedTargetDistance = 0.2f;
	[SerializeField]
	private float damageRadius = 4f;
	[SerializeField]
	private int damage = 30;
	private Vector3 targetPosition;
	private Action onGrenadeBehaviourComplete;
	
	
	private void Update()
	{
		Vector3 moveDir = (targetPosition - transform.position).normalized;
		transform.position += moveDir * moveSpeed * Time.deltaTime;
		
		if(Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance)
		{
			Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
			
			foreach(Collider collider in colliderArray)
			{
				if(collider.TryGetComponent<Unit>(out Unit targetUnit))
				{
					targetUnit.Damage(damage);
				}
			}
			
			
			onGrenadeBehaviourComplete?.Invoke();
			Destroy(gameObject);
		}
	}
	
	public void Setup(GridPosition targetGridPosition, Action OnGrenadeBehaviourComplete)
	{
		targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
		onGrenadeBehaviourComplete = OnGrenadeBehaviourComplete;
	}
}
