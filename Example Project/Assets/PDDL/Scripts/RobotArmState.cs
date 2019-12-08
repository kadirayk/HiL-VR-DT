using UnityEngine;

public class RobotArmState
{
	private float baseAngle;
	private float l2Angle;
	private float l3Angle;
	private Vector3 endEffectorPosition;
	private bool suctionActive;


	public RobotArmState(float baseAngle, float l2Angle, float l3Angle, bool suctionActive, Vector3 endEffectorPosition)
	{
		this.baseAngle = baseAngle;
		this.l2Angle = l2Angle;
		this.l3Angle = l3Angle;
		this.suctionActive = suctionActive;
		this.endEffectorPosition = endEffectorPosition;
	}

	public float BaseAngle { get => baseAngle; set => baseAngle = value; }
	public float L3Angle { get => l3Angle; set => l3Angle = value; }
	public float L2Angle { get => l2Angle; set => l2Angle = value; }
	public bool SuctionActive { get => suctionActive; set => suctionActive = value; }
	public Vector3 EndEffectorPosition { get => endEffectorPosition; set => endEffectorPosition = value; }
}
