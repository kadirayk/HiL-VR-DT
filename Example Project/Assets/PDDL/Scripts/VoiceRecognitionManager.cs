﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VoiceRecognitionManager : MonoBehaviour
{
	KeywordRecognizer keywordRecognizer;
	Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
	public GameObject Cube;
	

	private void Start()
	{
		Debug.Log("voice mananger active");
		//Create keywords for keyword recognizer
		/*keywords.Add("Alpha", () =>
		{
			Instantiate(Cube, new Vector3(0.25f, 0.025f, -0.0125f), Quaternion.identity);
		});

		keywords.Add("Bravo", () =>
		{
			Instantiate(Cube, new Vector3(0, 0.025f, 0.23f), Quaternion.identity);
		});

		keywords.Add("Charlie", () =>
		{
			Instantiate(Cube, new Vector3(-0.25f, 0.025f, -0.0125f), Quaternion.identity);
		});*/
		MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
		keywords.Add("Record", () =>
		{
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Recording Movements";
			mr.StartRecording();
		});

		keywords.Add("Stop", () =>
		{
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Recording Stopped";
			Debug.Log("stop");
			mr.StopRecording();
		});

		keywords.Add("Replay", () =>
		{
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Replaying Movements";
			Debug.Log("replaying");
			mr.Replay();
		});

		keywords.Add("Initial", () =>
		{
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Initial State";
			WorldState ws = GameObject.FindObjectOfType<WorldState>();
			ws.Initial();
		});

		keywords.Add("Goal", () =>
		{
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Goal State";
			WorldState ws = GameObject.FindObjectOfType<WorldState>();
			ws.Goal();
		});

		keywords.Add("Solve", () =>
		{
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Solving";
			WorldState ws = GameObject.FindObjectOfType<WorldState>();
			ws.Solve();
		});


		keywords.Add("Restart", () =>
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		});

		keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), ConfidenceLevel.Low);

		keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
		keywordRecognizer.Start();
	}

	private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
	{
		Debug.Log(args.text + " " + args.confidence);
		System.Action keywordAction;
		// if the keyword recognized is in our dictionary, call that Action.
		if (keywords.TryGetValue(args.text, out keywordAction))
		{
			keywordAction.Invoke();
		}
	}

	/*
	[SerializeField]
	private string[] m_Keywords;

	private KeywordRecognizer m_Recognizer;
	public GameObject Cube;
	public GameObject Sphere;

    // Start is called before the first frame update
    void Start()
    {
		m_Keywords = new string[1];
		m_Keywords[0] = "Cube";
		m_Keywords[1] = "at";
		m_Keywords[2] = "A";
		m_Keywords[2] = "B";
		m_Keywords[2] = "A";
		m_Recognizer = new KeywordRecognizer(m_Keywords);
		m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
		m_Recognizer.Start();
      
    }

	private void OnPhraseRecognized(PhraseRecognizedEventArgs args) {
		/*Vector3 positionA = new Vector3(0.22f, 0, 0.05f);
		Vector3 positionB = new Vector3(0, 0.22f, 0.05f);
		Vector3 positionC = new Vector3(-0.22f, 0, 0.05f);

		if (args.text == m_Keywords[0]) {
			Instantiate(Cube, new Vector3(newX, newZ, 0.05f), Quaternion.identity);
		}
	Debug.Log("We recognized:" + args.text + "meaning: " + args.semanticMeanings + " with confidence:" + args.confidence);
	}
*/
}
