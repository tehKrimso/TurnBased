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
	
	private void Start()
	{
		UpdateNumberText();
		
		endTurnButton.onClick.AddListener(() => 
		{
			TurnSystem.Instance.NextTurn();
		});
		
		TurnSystem.Instance.OnTurnChganged += TurnSystem_OnTurnChanged;
	}
	
	
	
	private void UpdateNumberText()
	{
		turnNumberText.text = $"TURN {TurnSystem.Instance.GetCurrentTurnNumber()}";
	}
	
	private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
	{
		UpdateNumberText();
	}
}
