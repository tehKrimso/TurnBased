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
	[SerializeField]
	private Transform rifleTransform;
	[SerializeField]
	private Transform swordTransform;
	
	private void Awake()
	{
		if(TryGetComponent<MoveAction>(out MoveAction moveAction))
		{
			moveAction.OnStartMoving += MoveAction_OnStartMoving;
			moveAction.OnStopMoving += MoveAction_OnStopMoving;
			moveAction.OnChangeFloorsStart += MoveAction_OnChangeFloorsStart;
		}
		
		if(TryGetComponent<ShootAction>(out ShootAction shootAction))
		{
			shootAction.OnShoot += ShootAction_OnShoot;
		}
		
		if(TryGetComponent<SwordAction>(out SwordAction swordAction))
		{
			swordAction.OnSwordActionStarted += swordAction_OnSwordActionStarted;
			swordAction.OnSwordActionCompleted += swordAction_OnSwordActionCompleted;
		}
	}

	private void MoveAction_OnChangeFloorsStart(object sender, MoveAction.OnChangeFloorsStartEventArgs e)
	{
		if(e.targetGridPosition.floor > e.unitGridPosition.floor)
		{
			animator.SetTrigger("JumpUp");
		}
		else
		{
			animator.SetTrigger("JumpDown");
		}
	}

	private void Start()
	{
		EquipRifle();
	}

	private void swordAction_OnSwordActionCompleted(object sender, EventArgs e)
	{
		EquipRifle();
	}

	private void swordAction_OnSwordActionStarted(object sender, EventArgs e)
	{
		EquipSword();
		animator.SetTrigger("SwordSlash");
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
	
	private void EquipSword()
	{
		rifleTransform.gameObject.SetActive(false);
		swordTransform.gameObject.SetActive(true);
	}
	
	private void EquipRifle()
	{
		swordTransform.gameObject.SetActive(false);
		rifleTransform.gameObject.SetActive(true);
	}

}
