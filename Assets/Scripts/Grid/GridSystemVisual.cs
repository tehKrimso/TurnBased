using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
	public static GridSystemVisual Instance { get; private set;}
	
	[SerializeField]
	private Transform gridSystemVisualPrefab;
	
	private GridSystemVisualSingle[,] gridSystemVisualSingleArray;
	
	private void Awake()
	{
		if(Instance != null)
		{
			Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		
		Instance = this;
	}
	
	private void Start() 
	{
		gridSystemVisualSingleArray = new GridSystemVisualSingle[
			LevelGrid.Instance.GetWidth(),
			LevelGrid.Instance.GetHeight()
		];
		
		for(int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
		{
			for(int z = 0; z < LevelGrid.Instance.GetWidth(); z++)
			{
				Transform gridSystemVisualSingleTransform = 
					Instantiate(gridSystemVisualPrefab, LevelGrid.Instance.GetWorldPosition(new GridPosition(x,z)),Quaternion.identity);
				
				gridSystemVisualSingleArray[x,z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
			}
		}
		
		HideAllGridPosition();
	}
	
	private void Update()
	{
		UpdateGridVisual();
	}
	
	public void HideAllGridPosition()
	{
		foreach(GridSystemVisualSingle gridVisual in gridSystemVisualSingleArray)
		{
			gridVisual.Hide();
		}
	}
	
	public void ShowGridPositionList(List<GridPosition> gridPositions)
	{
		foreach(GridPosition gridPos in gridPositions)
		{
			gridSystemVisualSingleArray[gridPos.x, gridPos.z].Show();
		}
	}
	
	private void UpdateGridVisual()
	{
		Unit unit = UnitActionSystem.Instance.GetSelectedUnit();
		
		HideAllGridPosition();
		List<GridPosition> validPositions = unit.GetMoveAction().GetValidActionGridPositionList();
		ShowGridPositionList(validPositions);
	}
}
