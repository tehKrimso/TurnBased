using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	private enum State
	{
		WaitingForEnemyTurn,
		TakingTurn,
		Busy,
	}
	
	private State state;
	private float timer;
	
	private void Awake()
	{
		state = State.WaitingForEnemyTurn;
	}
	
	private void Start()
	{
		TurnSystem.Instance.OnTurnChganged += TurnSystem_OnTurnChganged;
	}
	
	private void Update()
	{
		if(TurnSystem.Instance.IsPlayerTurn())
		{
			return;
		}
		
		switch (state)
		{
			case State.WaitingForEnemyTurn:
				break;
			case State.TakingTurn:
				timer -= Time.deltaTime;
				if(timer <= 0f)
				{
					if(TryTakeEnemyAIAction(SetStateTakingTurn))
					{
						state = State.Busy;
					}
					else
					{
						TurnSystem.Instance.NextTurn();
					}
					
					
				}
				break;
			case State.Busy:
				break;
		}
		
		
	}
	
	private void SetStateTakingTurn()
	{
		timer = 0.5f;
		state = State.TakingTurn;
	}
	
	private void TurnSystem_OnTurnChganged(object sender, EventArgs e)
	{
		if(!TurnSystem.Instance.IsPlayerTurn())
		{
			state = State.TakingTurn;
			timer = 2f;
		}
	}
	
	private bool TryTakeEnemyAIAction(Action onActionComplete)
	{
		foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
		{
			if(TryTakeEnemyAIAction(enemyUnit, onActionComplete))
			{
				return true;
			}
			
		}
		
		return false;
	}
	
	private bool TryTakeEnemyAIAction(Unit enemyUnit,Action onEnemyAIActionComplete)
	{
		EnemyAIAction bestEnemyAIAction = null;
		BaseAction bestBaseAction = null;
		foreach(BaseAction baseAction in enemyUnit.GetBaseActionArray())
		{
			if(!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
			{
				//can not affrod action
				continue;
			}
			
			if(bestEnemyAIAction == null)
			{
				bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
				bestBaseAction = baseAction;
			}
			else
			{
				EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
			
				if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
				{
					bestEnemyAIAction = testEnemyAIAction;
					bestBaseAction = baseAction;
				}
			}
		}
		
		if(bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
		{
			bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
			return true;
		}
		else
		{
			return false;
		}
	}
}
