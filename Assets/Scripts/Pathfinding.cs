using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
	public static Pathfinding Instance { get; private set;}
	private const int MOVE_STRAIGHT_COST = 10;
	private const int MOVE_DIAGONAL_COST = 14;
	
	[SerializeField]
	private Transform gridDebugObjectPrefab;
	[SerializeField]
	private LayerMask obstaclesLayerMask;
	[SerializeField]
	private LayerMask floorLayerMask;
	[SerializeField]
	private Transform pathfindingLinkContainer;
	private int width;
	private int height;
	private float cellSize;
	private int floorAmount;
	
	private List<GridSystem<PathNode>> gridSystemList;
	private List<PathfindingLink> pathfindingLinkList;
	
	private void Awake()
	{
		if(Instance != null)
		{
			Debug.LogError("There's more than one Pathfinding! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		
		Instance = this;
	}
	
	public void Setup(int width, int height, float cellSize, int floorAmount)
	{
		this.width = width;
		this.height = height;
		this.cellSize = cellSize;
		this.floorAmount = floorAmount;
		
		gridSystemList = new List<GridSystem<PathNode>>();
		
		for(int floor = 0 ;floor < floorAmount; floor++)
		{
			GridSystem<PathNode> gridSystem = 
			new GridSystem<PathNode>(width,height, cellSize, floor, LevelGrid.FLOOR_HEIGHT, 
			(GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
			
			gridSystemList.Add(gridSystem);
		}
		
		//gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
		
		for (int x = 0; x < width; x++)
		{
			for( int z = 0; z < height; z++)
			{
				for(int floor = 0; floor < floorAmount; floor++)
				{
					GridPosition gridPosition = new GridPosition(x,z,floor);
					Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
					float raycastOffsetDistance = 1f;
					
					PathNode pathNode = GetNode(x,z, floor);
					pathNode.SetIsWalkable(false);
					
					if(Physics.Raycast(
						worldPosition + Vector3.down * raycastOffsetDistance,
						Vector3.up,
						raycastOffsetDistance * 2,
						obstaclesLayerMask))
						{
							pathNode.SetIsWalkable(false);
							continue;
						}
						
						
					if(Physics.Raycast(
						worldPosition + Vector3.up * raycastOffsetDistance,
						Vector3.down,
						raycastOffsetDistance * 2,
						floorLayerMask))
						{
							pathNode.SetIsWalkable(true);
						}	 
				}
				
			}
		}
		
		pathfindingLinkList = new List<PathfindingLink>();
		
		foreach(Transform pathfindingLinkTransform in pathfindingLinkContainer)
		{
			
			if(pathfindingLinkTransform.TryGetComponent(out PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour))
			{
				pathfindingLinkList.Add(pathfindingLinkMonoBehaviour.GetPathfindingLink());
			}
		}
	}
	
	public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
	{
		List<PathNode> openList = new List<PathNode>();
		List<PathNode> closedList = new List<PathNode>();
		
		PathNode startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition);
		PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);
		
		openList.Add(startNode);
		
		for (int x = 0; x < width; x++)
		{
			for (int z = 0; z<height; z++)
			{
				for(int floor = 0; floor < floorAmount; floor++)
				{
					GridPosition gridPosition = new GridPosition(x,z,floor);
					PathNode pathNode = GetGridSystem(floor).GetGridObject(gridPosition);
				
					pathNode.SetGCost(int.MaxValue);
					pathNode.SetHCost(0);
					pathNode.CalculateFCost();
					pathNode.ResetCameFromPathNode();
				}
			}
		}
		
		startNode.SetGCost(0);
		startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
		startNode.CalculateFCost();
		
		while(openList.Count > 0)
		{
			PathNode currentNode = GetLowestFCostPathNode(openList);
			if(currentNode == endNode)
			{
				//reached final node
				pathLength = endNode.GetFCost();
				return CalculatePath(endNode);
			}
			
			openList.Remove(currentNode);
			closedList.Add(currentNode);
			
			foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
			{
				if(closedList.Contains(neighbourNode))
				{
					continue;
				}
				
				if(!neighbourNode.IsWalkable())
				{
					closedList.Add(neighbourNode);
					continue;
				}
				
				int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());
				
				if(tentativeGCost < neighbourNode.GetGCost())
				{
					neighbourNode.SetCameFromPathNode(currentNode);
					neighbourNode.SetGCost(tentativeGCost);
					neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
					neighbourNode.CalculateFCost();
					
					if(!openList.Contains(neighbourNode))
					{
						openList.Add(neighbourNode);
					}
				}
			}
		}
		
		//No path found
		pathLength = 0;
		return null;
		
	}
	
	
	public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
	{
		GridPosition gridPositionDistance = gridPositionA - gridPositionB;
		int xDistance = Mathf.Abs(gridPositionDistance.x);
		int zDistance = Mathf.Abs(gridPositionDistance.z);
		int remaining = Mathf.Abs(xDistance - zDistance);
		return MOVE_DIAGONAL_COST * Mathf.Min(xDistance,zDistance) + MOVE_STRAIGHT_COST * remaining;
	}
	
	private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
	{
		PathNode lowestFCostPathNode = pathNodeList[0];
		for (int i = 0; i < pathNodeList.Count; i++)
		{
			if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
			{
				lowestFCostPathNode = pathNodeList[i];
			}
		}

		
		return lowestFCostPathNode;
	}
	
	private GridSystem<PathNode> GetGridSystem(int floor) => gridSystemList[floor];
	
	private PathNode GetNode(int x, int z, int floor)
	{
		return GetGridSystem(floor).GetGridObject(new GridPosition(x,z,floor));
	}
	
	private List<PathNode> GetNeighbourList(PathNode currentNode)
	{
		List<PathNode> neighbourList = new List<PathNode>();
		
		GridPosition gridPosition = currentNode.GetGridPosition();
		
		if(gridPosition.x - 1 >= 0)
		{
			//Left
			neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z,gridPosition.floor));
			
			if(gridPosition.z - 1 >= 0)
			{
				//LeftDown
				neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1,gridPosition.floor));
			}
			
			if(gridPosition.z + 1 < height)
			{
				//LeftUp
				neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1,gridPosition.floor));
			}
		}
		
		if(gridPosition.x + 1 < width)
		{
			//Right
			neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z,gridPosition.floor));
			
			if(gridPosition.z - 1 >= 0)
			{
				//RightDown
				neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1,gridPosition.floor));
			}
			
			if(gridPosition.z + 1 < height)
			{
				//RightUp
				neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1,gridPosition.floor));
			}
		}
		
		if(gridPosition.z - 1 >= 0)
		{
			//Down
			neighbourList.Add(GetNode(gridPosition.x, gridPosition.z - 1,gridPosition.floor));
		}
		
		if(gridPosition.z + 1 < height)
		{
			//Up
			neighbourList.Add(GetNode(gridPosition.x, gridPosition.z + 1,gridPosition.floor));
		}
		
		List<PathNode> totalNeighbourList = new List<PathNode>();
		totalNeighbourList.AddRange(neighbourList);
		
		List<GridPosition> pathfindingLinkGridPositionList = GetPathfindingLinkConnectedGridPositionList(gridPosition);
		
		foreach(GridPosition pathfindingLinkGridPosition in pathfindingLinkGridPositionList)
		{
			totalNeighbourList.Add(
				GetNode(
					pathfindingLinkGridPosition.x,
					pathfindingLinkGridPosition.z,
					pathfindingLinkGridPosition.floor
				)
			);
		}
		
		return totalNeighbourList;
	}
	
	private List<GridPosition> GetPathfindingLinkConnectedGridPositionList(GridPosition gridPosition)
	{
		List<GridPosition> gridPositionList = new List<GridPosition>();
		
		foreach(PathfindingLink pathfindingLink in pathfindingLinkList)
		{
			if(pathfindingLink.gridPositionA == gridPosition)
			{
				gridPositionList.Add(pathfindingLink.gridPositionB);
			}
			
			if(pathfindingLink.gridPositionB == gridPosition)
			{
				gridPositionList.Add(pathfindingLink.gridPositionA);
			}
		}
		
		return gridPositionList;
	}
	
	private List<GridPosition> CalculatePath(PathNode endNode)
	{
		List<PathNode> pathNodeList = new List<PathNode>();
		pathNodeList.Add(endNode);
		PathNode currentNode = endNode;
		while(currentNode.GetCameFromPathNode() != null)
		{
			pathNodeList.Add(currentNode.GetCameFromPathNode());
			currentNode = currentNode.GetCameFromPathNode();
		}
		
		pathNodeList.Reverse();
		
		return pathNodeList.Select(n => n.GetGridPosition()).ToList();
	}
	
	public bool IsWalkableGridPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
	
	public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable) => GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
	
	public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
	{
		return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
	}
	
	public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
	{
		FindPath(startGridPosition, endGridPosition, out int pathLength);
		return pathLength;
	}
	
	
}
