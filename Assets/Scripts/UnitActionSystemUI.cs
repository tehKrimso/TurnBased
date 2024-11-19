using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
	[SerializeField]
	private Transform actionButtonPrefab;
	
	[SerializeField]
	private Transform actionButtonContainerTransform;
	
	private void Start() 
	{
		UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitchanged;
		CreateUnitActionButtons();	
	}
	
	private void CreateUnitActionButtons()
	{
		foreach(Transform buttonTransform in actionButtonContainerTransform)
		{
			Destroy(buttonTransform.gameObject);
		}
		
		Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
		
		foreach(BaseAction baseAction in selectedUnit.GetBaseActionArray())
		{
			Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
			ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
			actionButtonUI.SetBaseAction(baseAction);
		}
	}
	
	private void UnitActionSystem_OnSelectedUnitchanged(object sender, EventArgs e)
	{
		CreateUnitActionButtons();
	}
}
