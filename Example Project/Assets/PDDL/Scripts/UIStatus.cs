using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIStatus : MonoBehaviour
{
	[Header("Left Panel")]
	public Button status;
	public Button debugSolution;

	[Header("Right Panel")]
	public Button debugInitial;
	public Button debugGoal;


	public void setStatus(string text)
	{
		status.GetComponentInChildren<Text>().text = text;
	}

	public void setSolution(string text)
	{
		debugSolution.GetComponentInChildren<Text>().text = text;
	}

	public void setInitial(string text) {
		debugInitial.GetComponentInChildren<Text>().text = text;
	}

	public void setGoal(string text)
	{
		debugGoal.GetComponentInChildren<Text>().text = text;
	}

}
