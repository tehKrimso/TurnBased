using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
	[SerializeField]
	private float explosionForce = 300f;
	
	[SerializeField]
	private float explosionRadius = 10f;
	
	[SerializeField]
	private Transform ragdollRootBone;
	
	private Transform originalRootBone;
	
	public void Setup(Transform originalRootBone)
	{
		this.originalRootBone = originalRootBone;	
		MatchAllChildTransforms(originalRootBone, ragdollRootBone);
		ApplyExplosionToRagdoll(ragdollRootBone, explosionForce, transform.position, explosionRadius);
	}
	
	private void MatchAllChildTransforms(Transform root, Transform clone)
	{
		foreach(Transform child in root)
		{
			Transform cloneChild = clone.Find(child.name);
			if(cloneChild != null)
			{
				cloneChild.position = child.position;
				cloneChild.rotation = child.rotation;
			}
			
			MatchAllChildTransforms(child, cloneChild);
		}
	}
	
	private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRadius)
	{
		foreach(Transform child in root)
		{
			if(child.TryGetComponent<Rigidbody>(out Rigidbody childRigidBody))
			{
				childRigidBody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
			}
			
			ApplyExplosionToRagdoll(child,explosionForce, explosionPosition, explosionRadius);
		}
	}
	
}
