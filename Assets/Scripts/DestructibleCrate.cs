using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestructibleCrate : MonoBehaviour
{
	[SerializeField]
	private Transform crateDestroyedPrefab;
	public static event EventHandler OnAnyDestroyed;
	private GridPosition gridPosition;
	
	private void Start() 
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
	}
	public void Damage()
	{
		Transform crateDestroyedTransfrom = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);
		ApplyExplosionToChildren(crateDestroyedTransfrom,150f, transform.position, 10f);
		
		OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
		Destroy(gameObject);
	}
	
	public GridPosition GetGridPosition() => gridPosition;
	
	private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRadius)
	{
		foreach(Transform child in root)
		{
			if(child.TryGetComponent<Rigidbody>(out Rigidbody childRigidBody))
			{
				childRigidBody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
			}
			
			ApplyExplosionToChildren(child,explosionForce, explosionPosition, explosionRadius);
		}
	}
}
