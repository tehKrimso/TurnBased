using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI testMeshPro;
	
	[SerializeField]
	private Button button;
	
	[SerializeField]
	private GameObject selectedGameObject;
	
	private BaseAction baseAction;
	
	public void SetBaseAction(BaseAction baseAction)
	{
		this.baseAction = baseAction;
		
		testMeshPro.text = baseAction.GetActionName().ToUpper();
		
		button.onClick.AddListener(() => 
		{
			UnitActionSystem.Instance.SetSelectedAction(baseAction);
		});
	}
	
	public void UpdateSelectedVisual()
	{
		BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
		selectedGameObject.SetActive(selectedAction == baseAction);
	}
	
}
