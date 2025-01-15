using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
	public static event EventHandler OnAnyGrenadeExploded;
	
	[SerializeField]
	private float moveSpeed = 15f;
	[SerializeField]
	private float reachedTargetDistance = 0.2f;
	[SerializeField]
	private float damageRadius = 4f;
	[SerializeField]
	private int damage = 30;
	[SerializeField]
	private Transform grenadeExplodeVfxPrefab;
	[SerializeField]
	private TrailRenderer trailRenderer;
	[SerializeField]
	private AnimationCurve arcYAnimationCurve;
	private Vector3 targetPosition;
	private Action onGrenadeBehaviourComplete;
	private float totalDistaance;
	private Vector3 positionXZ;
	
	
	private void Update()
	{
		Vector3 moveDir = (targetPosition - positionXZ).normalized;
		positionXZ += moveDir * moveSpeed * Time.deltaTime;
		
		float distance = Vector3.Distance(positionXZ, targetPosition);
		float distanceNormalized = 1-  distance / totalDistaance;
		
		float maxHeight = totalDistaance / 4f;
		float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
		
		transform.position = new Vector3(positionXZ.z, positionY, positionXZ.z);
		
		if(Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
		{
			Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
			
			foreach(Collider collider in colliderArray)
			{
				if(collider.TryGetComponent<Unit>(out Unit targetUnit))
				{
					targetUnit.Damage(damage);
				}
				
				if(collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate crate))
				{
					crate.Damage();
				}
			}
			
			
			onGrenadeBehaviourComplete?.Invoke();
			OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
			trailRenderer.transform.parent = null;
			Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
			Destroy(gameObject);
		}
	}
	
	public void Setup(GridPosition targetGridPosition, Action OnGrenadeBehaviourComplete)
	{
		targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
		onGrenadeBehaviourComplete = OnGrenadeBehaviourComplete;
		
		positionXZ = transform.position;
		positionXZ.y = 0;
		totalDistaance = Vector3.Distance(positionXZ, targetPosition);
	}
}
