using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
	// tab selectors
	[Header("Tab Selectors")]
	public Button manualButton;
	public Button automatedButton;

	[Header("Manual Panel")]
	public Button recordButton;
	public Button stopButton;
	public Button replayButton;
	public Button suctionButton;

	[Header("Automated Panel")]
	public Button registerInitialButton;
	public Button registerGoalButton;
	public Button solveButton;

	[Header("Common Panel")]
	public Button restartButton;
	public Button executeButton;

	private bool isManualActive = true;
	UIStatus uiStatus;


	GameObject manualPanel;
	GameObject automatedPanel;

	MovementRecorder movementRecorder;
	WorldState worldState;
	EndEffectorCollisionController cd;

	void Start()
	{
		uiStatus = GameObject.FindObjectOfType<UIStatus>();
		manualPanel = GameObject.Find("ManualPanel");
		automatedPanel = GameObject.Find("AutomatedPanel");

		manualButton.GetComponent<Button>().onClick.AddListener(ManualButtonOnClick);
		automatedButton.GetComponent<Button>().onClick.AddListener(AutomatedButtonOnClick);
		
		recordButton.GetComponent<Button>().onClick.AddListener(RecordButtonOnClick);
		stopButton.GetComponent<Button>().onClick.AddListener(StopButtonOnClick);
		replayButton.GetComponent<Button>().onClick.AddListener(ReplayButtonOnClick);
		suctionButton.GetComponent<Button>().onClick.AddListener(SuctionButtonOnClick);

		registerInitialButton.GetComponent<Button>().onClick.AddListener(RegisterInitialButtonOnClick);
		registerGoalButton.GetComponent<Button>().onClick.AddListener(RegisterGoalButtonOnClick);
		solveButton.GetComponent<Button>().onClick.AddListener(SolveButtonOnClick);
		
		restartButton.GetComponent<Button>().onClick.AddListener(RestartButtonOnClick);
		executeButton.GetComponent<Button>().onClick.AddListener(ExecuteButtonOnClick);

		movementRecorder = GameObject.FindObjectOfType<MovementRecorder>();
		worldState = GameObject.FindObjectOfType<WorldState>();
		cd = GameObject.FindObjectOfType<EndEffectorCollisionController>();


	}

	void Update()
	{
		
		if (isManualActive)
		{
			manualPanel.SetActive(true);
			automatedPanel.SetActive(false);
		}
		else
		{
			manualPanel.SetActive(false);
			automatedPanel.SetActive(true);
		}

		if (cd.isSuctionActive())
		{
			suctionButton.GetComponentInChildren<Text>().text = "Stop Suction";
			ColorBlock colors = suctionButton.colors;
			colors.normalColor = Color.red;
			suctionButton.colors = colors;
		}
		else {
			suctionButton.GetComponentInChildren<Text>().text = "Start Suction";
			ColorBlock colors = suctionButton.colors;
			colors.normalColor = Color.green;
			suctionButton.colors = colors;
		}


	}

	void ManualButtonOnClick()
	{
		isManualActive = true;
	}

	void AutomatedButtonOnClick()
	{
		isManualActive = false;
	}

	void RecordButtonOnClick()
	{
		worldState.Initial();
		uiStatus.setStatus("Recording Movements");
		movementRecorder.StartRecording();
	}

	void StopButtonOnClick()
	{
		uiStatus.setStatus("Recording Stopped");
		movementRecorder.StopRecording();
	}

	void ReplayButtonOnClick()
	{
		worldState.ToInitialPosition();
		uiStatus.setStatus("Replaying Movements");
		movementRecorder.Replay();
	}


	void SuctionButtonOnClick()
	{
		if (suctionButton.GetComponentInChildren<Text>().text.Equals("Stop Suction"))
		{
			cd.setSuction(false);
		}
		else {
			cd.setSuction(true);
		}
	}

	void RegisterInitialButtonOnClick()
	{
	}

	void RegisterGoalButtonOnClick()
	{
		worldState.Goal();
	}

	void SolveButtonOnClick()
	{
		worldState.Solve();
	}

	void RestartButtonOnClick()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void ExecuteButtonOnClick()
	{
		if (isManualActive) {
			movementRecorder.Execute();
		}
		else
		{
			worldState.Execute();
		}
		
		uiStatus.setStatus("Executing");
	}
}
