using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RoboticSystemState
{
	RobotArmState dobotLoaderState;
	RobotArmState dobotRailState;
	bool conveyorState;

	public RoboticSystemState(RobotArmState dobotLoaderState, RobotArmState dobotRailState, bool conveyorState)
	{
		this.dobotLoaderState = dobotLoaderState;
		this.dobotRailState = dobotRailState;
		this.conveyorState = conveyorState;
	}

	public RobotArmState DobotLoaderState { get => dobotLoaderState; set => dobotLoaderState = value; }
	public RobotArmState DobotRailState { get => dobotRailState; set => dobotRailState = value; }

	public bool ConveyorState { get => conveyorState; set => conveyorState = value; }

}
