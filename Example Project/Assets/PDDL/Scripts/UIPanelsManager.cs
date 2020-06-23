using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelsManager : MonoBehaviour
{
	public Button manualTabButton;
	public Button automatedTabButton;

	void Start()
	{
		Button manual = manualTabButton.GetComponent<Button>();
		manual.onClick.AddListener(ManualTabOnClick);

		Button automated = automatedTabButton.GetComponent<Button>();
		automated.onClick.AddListener(AutomatedTabOnClick);
		
	}

	void ManualTabOnClick()
	{
		Debug.Log("You have clicked the manual!");
	}

	void AutomatedTabOnClick()
	{
		Debug.Log("You have clicked the automated!");
	}

}
