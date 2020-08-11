using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using RosSharp.RosBridgeClient;
using System.Collections.Generic;

public enum ButtonActivation
{
	manual_record,
	manual_stop,
	manual_replay,
	auto_show,
	auto_registerGoal,
	auto_solve,
	execute,
	restart
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
	public GameObject Sphere;
	HintsManager hintsManager;


	GameObject manualPanel;
	GameObject automatedPanel;
	GameObject debugPanel;

	MovementRecorder movementRecorder;
	WorldState worldState;
	EndEffectorCollisionController cd;
	ModeManager modeManager;

	Dictionary<Button, Color> normalColors = new Dictionary<Button, Color>();

	private Button buttonToHighLight;



	void Start()
	{
		uiStatus = GameObject.FindObjectOfType<UIStatus>();
		hintsManager = GameObject.FindObjectOfType<HintsManager>();
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
		
		if (isManualActive)
		{
			modeManager.SetMode(Mode.manual);
			buttonToHighLight = recordButton;
			hintsManager.setStatus(HintsManager.MANUAL_INITIAL);
		}
		else
		{
			modeManager.SetMode(Mode.automated);
			buttonToHighLight = registerGoalButton;
			hintsManager.setStatus(HintsManager.AUTO_INITIAL);
		}
	}

	void Update()
	{
		highLightButton();
		//automated vs manual switch
		manualPanel.SetActive(isManualActive);
		automatedPanel.SetActive(!isManualActive);


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
		buttonToHighLight = recordButton;
		modeManager.SetMode(Mode.manual);
		hintsManager.setStatus(HintsManager.MANUAL_INITIAL);
	}

	void AutomatedButtonOnClick()
	{
		isManualActive = false;
		buttonToHighLight = registerGoalButton;
		modeManager.SetMode(Mode.automated);
		hintsManager.setStatus(HintsManager.AUTO_INITIAL);
	}

	void RecordButtonOnClick()
	{
		uiStatus.setStatus("Recording Movements");
		worldState.Initial();
		movementRecorder.StartRecording();
		buttonToHighLight = stopButton;
		Sphere.SetActive(true);
		hintsManager.setStatus(HintsManager.RECORDING_PRESSED);

	}

	void StopButtonOnClick()
	{
		uiStatus.setStatus("Recording Stopped");
		movementRecorder.StopRecording();
		buttonToHighLight = replayButton;
		hintsManager.setStatus(HintsManager.STOP_PRESSED);
	}

	void ReplayButtonOnClick()
	{
		worldState.ToInitialPosition();
		uiStatus.setStatus("Replaying Movements");
		Sphere.SetActive(false);
		movementRecorder.Replay();
		buttonToHighLight = executeButton;
		hintsManager.setStatus(HintsManager.REPLAY_PRESSED);
	}


	void SuctionButtonOnClick()
	{
		if (suctionButton.GetComponentInChildren<Text>().text.Equals("Stop Suction"))
		{
			cd.setSuction(false);
			hintsManager.setStatus(HintsManager.STOP_SUCTION_PRESSED);
		}
		else
		{
			cd.setSuction(true);
			if (!cd.isCollidingWithCube())
			{
				hintsManager.setStatus(HintsManager.SUCTION_WITHOUT_TOUCH);
			}
			else
			{
				hintsManager.setStatus(HintsManager.HOLDING_CUBE);
			}
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
		buttonToHighLight = solveButton;
		hintsManager.setStatus(HintsManager.AUTO_REGISTER_PRESSED);
	}

	void SolveButtonOnClick()
	{
		worldState.Solve();
		buttonToHighLight = executeButton;
		hintsManager.setStatus(HintsManager.SOLVE_PRESSED);
	}

	void RestartButtonOnClick()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		Sphere.SetActive(true);
	}

	void ExecuteButtonOnClick()
	{
		if (isManualActive)
		{
			movementRecorder.Execute();
			Debug.Log("manual active");
		}
		else
		{
			worldState.Execute();
			Debug.Log("automated active");
		}
		Sphere.SetActive(false);
		uiStatus.setStatus("Executing");
		hintsManager.setStatus(HintsManager.EXECUTE_PRESSED);
	}

	private void highLightButton()
	{
		if (buttonToHighLight != null)
		{
			ColorBlock colors = buttonToHighLight.colors;
			Color norm = colors.normalColor;
			float H, S, V;
			Color.RGBToHSV(norm, out H, out S, out V);
			S = Mathf.PingPong(Time.time, 0.5f);
			norm = Color.HSVToRGB(H, S, V);
			colors.normalColor = norm;
			buttonToHighLight.colors = colors;

			if (!buttonToHighLight.name.Equals(recordButton.name)) {
				unHighlightButton(recordButton);
			}
			if (!buttonToHighLight.name.Equals(stopButton.name))
			{
				unHighlightButton(stopButton);
			}
			if (!buttonToHighLight.name.Equals(replayButton.name))
			{
				unHighlightButton(replayButton);
			}
			if (!buttonToHighLight.name.Equals(registerGoalButton.name))
			{
				unHighlightButton(registerGoalButton);
			}
			if (!buttonToHighLight.name.Equals(solveButton.name))
			{
				unHighlightButton(solveButton);
			}
			if (!buttonToHighLight.name.Equals(executeButton.name))
			{
				unHighlightButton(executeButton);
			}
		}
	}

	private void unHighlightButton(Button b)
	{
		ColorBlock colors = b.colors;
		colors.normalColor = Color.white;
		b.colors = colors;
	}
}
