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
	
	public void SetBaseAction(BaseAction baseAction)
	{
		testMeshPro.text = baseAction.GetActionName().ToUpper();
		
		button.onClick.AddListener(() => 
		{
			UnitActionSystem.Instance.SetSelectedAction(baseAction);
		});
	}
	
	
}
