using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
	private void Start()
	{
		UnitActionSystem.Instance.OnBusyChange += (object sender, bool isBusy) => gameObject.SetActive(isBusy);
		gameObject.SetActive(false);
	}
}
