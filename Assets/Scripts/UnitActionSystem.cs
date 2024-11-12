using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
	[SerializeField]
	private Unit _selectedUnit;
	[SerializeField]
	private LayerMask _unitLayerMask;
	
	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			if(TryHandleUnitSelection()) return;
			
			_selectedUnit.Move(MouseWorld.GetPosition());
		}
	}
	
	private bool TryHandleUnitSelection()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _unitLayerMask))
		{
			if(raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
			{
				_selectedUnit = unit;
				return true;
			}
		}
		
		return false;
	}
}
