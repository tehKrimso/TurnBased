using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
	public static GridSystemVisual Instance { get; private set;}
	
	[Serializable]
	public struct GridVisualTypeMaterial
	{
		public GridVisualType gridVisualType;
		public Material material;
	}
	
	public enum GridVisualType
	{
		White,
		Blue,
		Red,
		RedSoft,
		Yellow
	}
	
	[SerializeField]
	private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
	
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
			for(int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
			{
				Transform gridSystemVisualSingleTransform = 
					Instantiate(gridSystemVisualPrefab, LevelGrid.Instance.GetWorldPosition(new GridPosition(x,z, 0)),Quaternion.identity);
				
				gridSystemVisualSingleArray[x,z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
			}
		}
		
		UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
		LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
		
		UpdateGridVisual();
	}

	private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
	{
		UpdateGridVisual();
	}

	private void UnitActionSystem_OnSelectedActionChange(object sender, EventArgs e)
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
	
	private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
	{
		List<GridPosition> gridPositionList = new List<GridPosition>();
		
		for (int x = -range; x<= range; x++)
		{
			for (int z = -range; z<= range; z++)
			{
				GridPosition testGridPosition = gridPosition + new GridPosition(x,z, 0);
				
				if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))//pos not inside grid
				{
					continue;
				}
				
				int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
				if(testDistance > range)
				{
					continue;
				}
				
				
				gridPositionList.Add(testGridPosition);
			}
		}
		
		ShowGridPositionList(gridPositionList, gridVisualType);
	}
	
	private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
	{
		List<GridPosition> gridPositionList = new List<GridPosition>();
		
		for (int x = -range; x<= range; x++)
		{
			for (int z = -range; z<= range; z++)
			{
				GridPosition testGridPosition = gridPosition + new GridPosition(x,z, 0);
				
				if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))//pos not inside grid
				{
					continue;
				}
				
				gridPositionList.Add(testGridPosition);
			}
		}
		
		ShowGridPositionList(gridPositionList, gridVisualType);
	}
	
	public void ShowGridPositionList(List<GridPosition> gridPositions, GridVisualType gridVisualType)
	{
		foreach(GridPosition gridPos in gridPositions)
		{
			gridSystemVisualSingleArray[gridPos.x, gridPos.z].Show(GetGridVisualTypeMaterial(gridVisualType));
		}
	}
	
	private void UpdateGridVisual()
	{
		BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
		
		HideAllGridPosition();
		List<GridPosition> validPositions = selectedAction.GetValidActionGridPositionList();
		
		
		GridVisualType gridVisualType;
		switch (selectedAction)
		{
			case MoveAction:
				gridVisualType = GridVisualType.White;
				break;
			case ShootAction shootAction:
				gridVisualType = GridVisualType.Red;
				ShowGridPositionRange(shootAction.GetUnit().GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
				break;
			case SpinAction:
				gridVisualType = GridVisualType.Blue;
				break;
			case GrenadeAction grenadeAction:
				gridVisualType = GridVisualType.Yellow;
				break;
			case SwordAction swordAction:
				gridVisualType = GridVisualType.Red;
				ShowGridPositionRangeSquare(swordAction.GetUnit().GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
				break;
			case InteractAction interactAction:
				gridVisualType = GridVisualType.Blue;
				break;
			default:
				gridVisualType = GridVisualType.White;
				break;
		}
		
		ShowGridPositionList(validPositions, gridVisualType);
		
		
	}
	
	private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
	{
		foreach(GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
		{
			if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
			{
				return gridVisualTypeMaterial.material;
			}
		}
		
		
		Debug.LogError("Could not find GridVisualMaterial for GridVisualType " + gridVisualType);
		return null;
	}
}
