using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public enum ManualState
{
	robotActing,
	init,
	recorded,
	stopped,
	replayed,
	executed
}

public enum AutomatedState
{
	robotActing,
	init,
	goalRegistered,
	solved,
	executed
}

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
	public Button showPDDLStatesButton;
	public Button registerGoalButton;
	public Button solveButton;

	[Header("Common Panel")]
	public Button restartButton;
	public Button executeButton;

	private bool isManualActive = true;
	private bool showPDDLDetails = false;
	UIStatus uiStatus;


	GameObject manualPanel;
	GameObject automatedPanel;
	GameObject debugPanel;

	MovementRecorder movementRecorder;
	WorldState worldState;
	EndEffectorCollisionController cd;
	ModeManager modeManager;

	ManualState manualState;
	AutomatedState automatedState;

	void Start()
	{
		uiStatus = GameObject.FindObjectOfType<UIStatus>();
		manualPanel = GameObject.Find("ManualPanel");
		automatedPanel = GameObject.Find("AutomatedPanel");
		debugPanel = GameObject.Find("DebugPanel");

		manualButton.GetComponent<Button>().onClick.AddListener(ManualButtonOnClick);
		automatedButton.GetComponent<Button>().onClick.AddListener(AutomatedButtonOnClick);

		recordButton.GetComponent<Button>().onClick.AddListener(RecordButtonOnClick);
		stopButton.GetComponent<Button>().onClick.AddListener(StopButtonOnClick);
		replayButton.GetComponent<Button>().onClick.AddListener(ReplayButtonOnClick);
		suctionButton.GetComponent<Button>().onClick.AddListener(SuctionButtonOnClick);

		showPDDLStatesButton.GetComponent<Button>().onClick.AddListener(ShowPDDLStatesButtonOnClick);
		registerGoalButton.GetComponent<Button>().onClick.AddListener(RegisterGoalButtonOnClick);
		solveButton.GetComponent<Button>().onClick.AddListener(SolveButtonOnClick);

		restartButton.GetComponent<Button>().onClick.AddListener(RestartButtonOnClick);
		executeButton.GetComponent<Button>().onClick.AddListener(ExecuteButtonOnClick);

		movementRecorder = GameObject.FindObjectOfType<MovementRecorder>();
		worldState = GameObject.FindObjectOfType<WorldState>();
		cd = GameObject.FindObjectOfType<EndEffectorCollisionController>();
		modeManager = GameObject.FindObjectOfType<ModeManager>();
		manualState = ManualState.init;
		automatedState = AutomatedState.init;
	}

	void Update()
	{
		//automated vs manual switch
		manualPanel.SetActive(isManualActive);
		automatedPanel.SetActive(!isManualActive);
		if (isManualActive)
		{
			modeManager.SetMode(Mode.manual);
		}
		else
		{
			modeManager.SetMode(Mode.automated);
		}
		

		// enable disable PDDL details
		if (showPDDLDetails)
		{
			debugPanel.SetActive(true);
			showPDDLStatesButton.GetComponentInChildren<Text>().text = "Hide PDDL States";
		}
		else
		{
			debugPanel.SetActive(false);
			showPDDLStatesButton.GetComponentInChildren<Text>().text = "Show PDDL States";
		}


		if (cd.isSuctionActive())
		{
			suctionButton.GetComponentInChildren<Text>().text = "Stop Suction";
			ColorBlock colors = suctionButton.colors;
			colors.normalColor = Color.red;
			suctionButton.colors = colors;
		}
		else
		{
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
		uiStatus.setStatus("Recording Movements");
		worldState.Initial();
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
		else
		{
			cd.setSuction(true);
		}
	}

	void ShowPDDLStatesButtonOnClick()
	{
		if (showPDDLStatesButton.GetComponentInChildren<Text>().text.Equals("Show PDDL States"))
		{
			showPDDLDetails = true;
		}
		else
		{
			showPDDLDetails = false;
		}
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
		if (isManualActive)
		{
			movementRecorder.Execute();
		}
		else
		{
			worldState.Execute();
		}

		uiStatus.setStatus("Executing");
	}
}
