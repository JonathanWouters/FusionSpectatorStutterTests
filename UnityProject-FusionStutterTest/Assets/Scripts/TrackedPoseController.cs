using UnityEngine;

public struct TrackedValues
{
	public Vector3 HmdPosition;
	public Quaternion HmdRotation;

	public Vector3 LeftControllerPosition;
	public Quaternion LeftControllerRotation;

	public Vector3 RightControllerPosition;
	public Quaternion RightControllerRotation;
}

public abstract class TrackedPoseController : MonoBehaviour
{
	public abstract TrackedValues GetTrackedValues();

}