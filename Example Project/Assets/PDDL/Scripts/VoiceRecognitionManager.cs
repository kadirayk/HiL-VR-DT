using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VoiceRecognitionManager : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
	KeywordRecognizer keywordRecognizer;
	Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
	public GameObject Cube;
#endif


	private void Start()
	{
#if UNITY_STANDALONE_WIN
		Debug.Log("voice mananger active");
		//Create keywords for keyword recognizer
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
#endif
	}

	#if UNITY_STANDALONE_WIN
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
#endif

	}
