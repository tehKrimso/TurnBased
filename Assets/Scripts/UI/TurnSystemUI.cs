using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI turnNumberText;
	[SerializeField]
	private Button endTurnButton;
	[SerializeField]
	private GameObject enemyTurnVisualGameObject;
	
	private void Start()
	{
		UpdateTurnText();
		UpdateEnemyTurnVisual();
		UpdateEndTurnButtonVisibility();
		
		endTurnButton.onClick.AddListener(() => 
		{
			TurnSystem.Instance.NextTurn();
		});
		
		TurnSystem.Instance.OnTurnChganged += TurnSystem_OnTurnChanged;
	}
	
	
	
	private void UpdateTurnText()
	{
		turnNumberText.text = $"TURN {TurnSystem.Instance.GetCurrentTurnNumber()}";
	}
	
	private void UpdateEnemyTurnVisual()
	{
		enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
	}
	
	private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
	{
		UpdateTurnText();
		UpdateEnemyTurnVisual();
		UpdateEndTurnButtonVisibility();
	}
	
	private void UpdateEndTurnButtonVisibility()
	{
		endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
	}
}
