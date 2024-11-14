using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
	[SerializeField]
	private Unit unit;
	
	private void Start()
	{
		
	}
	
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.T))
		{
			GridSystemVisual.Instance.HideAllGridPosition();
			List<GridPosition> validPositions = unit.GetMoveAction().GetValidActionGridPositionList();
			GridSystemVisual.Instance.ShowGridPositionList(validPositions);
		}
	}
}
