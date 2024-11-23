using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
	[SerializeField]
	private Unit _unit;
	private MeshRenderer _meshRenderer;
	
	private void Awake()
	{
		_meshRenderer = GetComponent<MeshRenderer>();
	}
	
	private void Start()
	{
		UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChanged;
		UpdateVisual();
	}
	
	private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
	{
		UpdateVisual();
	}
	
	private void UpdateVisual()
	{
		_meshRenderer.enabled = UnitActionSystem.Instance.GetSelectedUnit() == _unit;
	}
	
	private void OnDestroy() 
	{
		UnitActionSystem.Instance.OnSelectedUnitChange -= UnitActionSystem_OnSelectedUnitChanged;
	}
}
