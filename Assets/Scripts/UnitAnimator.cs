using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
	[SerializeField]
	private Animator animator;
	
	[SerializeField]
	private Transform bulletProjectilePrefab;
	
	[SerializeField]
	private Transform shootPointTransform;
	
	private void Awake()
	{
		if(TryGetComponent<MoveAction>(out MoveAction moveAction))
		{
			moveAction.OnStartMoving += MoveAction_OnStartMoving;
			moveAction.OnStopMoving += MoveAction_OnStopMoving;
		}
		
		if(TryGetComponent<ShootAction>(out ShootAction shootAction))
		{
			shootAction.OnShoot += ShootAction_OnShoot;

		}
	}

	private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
	{
		animator.SetTrigger("Shoot");
		
		Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
		
		Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
		targetUnitShootAtPosition.y = shootPointTransform.position.y;
		
		bulletProjectileTransform.GetComponent<BulletProjectile>().Setup(targetUnitShootAtPosition);
	}

	private void MoveAction_OnStartMoving(object sender, EventArgs e)
	{
		animator.SetBool("IsWalking", true);
	}
	
	private void MoveAction_OnStopMoving(object sender, EventArgs e)
	{
		animator.SetBool("IsWalking", false);
	}

}
