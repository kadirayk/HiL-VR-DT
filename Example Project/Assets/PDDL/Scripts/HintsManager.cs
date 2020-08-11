using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintsManager : MonoBehaviour
{

	public Button hints;

	public static string MANUAL_INITIAL = "Start recording.";
	public static string RECORDING_PRESSED = "You can move the robot arm now.";
	public static string MOVING_THE_ARM = "Place the arm to the center of a cube.\nStart suction only after the hand controller vibrates.";
	public static string HOLDING_CUBE = "Move the arm to a desired position and stop suction.";
	public static string STOP_SUCTION_PRESSED = "You can pick up a new cube now or stop recording if you are done.";
	public static string SUCTION_WITHOUT_TOUCH = "You have started suction without touching a cube. Stop suction now and start suction only after the hand controller vibrates";
	public static string STOP_PRESSED = "Replay the movements to check if you are satisfied with the recording.";
	public static string REPLAY_PRESSED = "Watch the robot replaying the movements. If you are satisfied with the replay, execute movements. If not, you can restart.";


	public static string AUTO_INITIAL = "Grab a cube and drop it to a desired position. If you are done, register the goal state";
	public static string AUTO_REGISTER_PRESSED = "Solve button makes the robot find a solution";
	public static string SOLVE_PRESSED = "Watch the robot performing a solution. If you are satisfied with the solution, execute. If not, you can restart";

	public static string EXECUTE_PRESSED = "Executing program. You can watch the real robot in camera feedback";

	public void setStatus(string text)
	{
		hints.GetComponentInChildren<Text>().text = text;
	}

}
