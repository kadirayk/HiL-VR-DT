using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class State
{
	public List<GameObject> PDDLObjectsAtPositionA;
	public List<GameObject> PDDLObjectsAtPositionB;
	public List<GameObject> PDDLObjectsAtPositionC;

	public State()
	{
		PDDLObjectsAtPositionA = new List<GameObject>();
		PDDLObjectsAtPositionB = new List<GameObject>();
		PDDLObjectsAtPositionC = new List<GameObject>();
	}

	public string toInitPDDL()
	{
		StringBuilder str = new StringBuilder("(:init ");
		foreach (GameObject obj in PDDLObjectsAtPositionA) {
			str.Append("(at ").Append(obj.name).Append(" posA)");
		}
		foreach (GameObject obj in PDDLObjectsAtPositionB)
		{
			str.Append("(at ").Append(obj.name).Append(" posB)");
		}
		foreach (GameObject obj in PDDLObjectsAtPositionC)
		{
			str.Append("(at ").Append(obj.name).Append(" posC)");
		}
		str.Append(")");
		return str.ToString();
	}

	public string toGoalPDDL()
	{
		// TODO: Maybe need to add AND before ats 
		StringBuilder str = new StringBuilder("(:goal ");
		foreach (GameObject obj in PDDLObjectsAtPositionA)
		{
			str.Append("(at ").Append(obj.name).Append(" posA)");
		}
		foreach (GameObject obj in PDDLObjectsAtPositionB)
		{
			str.Append("(at ").Append(obj.name).Append(" posB)");
		}
		foreach (GameObject obj in PDDLObjectsAtPositionC)
		{
			str.Append("(at ").Append(obj.name).Append(" posC)");
		}
		str.Append(")");
		return str.ToString();
	}

}