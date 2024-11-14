using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Unit : MonoBehaviour
{
	private GridPosition gridPosition;
	
	private MoveAction moveAction;
	
	
	private void Awake()
	{
		moveAction = GetComponent<MoveAction>();
	}
	
	private void Start()
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
	}
	
	private void Update()
	{
		
		
		GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		if(newGridPosition != gridPosition)
		{
			
			LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
			gridPosition = newGridPosition;
		}
	}
	
	public MoveAction GetMoveAction() => moveAction;
	
	public GridPosition GetGridPosition() => gridPosition;
}
