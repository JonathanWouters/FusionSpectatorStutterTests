using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.NetworkCharacterController;

[DefaultExecutionOrder(-999)]
public class CameraRig : MonoBehaviour
{
	public TrackedPoseController TrackedPoseController;
	public TrackedValues TrackedValues;

	public Transform Hmd;
	public Transform LeftHand;
	public Transform RightHand;

	public static CameraRig Instance { get; private set; }

	public void Awake()
	{
		Instance = this;
	}

	public void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	public void LateUpdate()
	{
		TrackedValues = TrackedPoseController.GetTrackedValues();

		Hmd.localPosition = TrackedValues.HmdPosition; 
		Hmd.localRotation = TrackedValues.HmdRotation;

		LeftHand.localPosition = TrackedValues.LeftControllerPosition; 
		LeftHand.localRotation = TrackedValues.LeftControllerRotation;

		RightHand.localPosition = TrackedValues.RightControllerPosition; 
		RightHand.localRotation = TrackedValues.RightControllerRotation;

		Vector3 movement = Vector3.zero;
		if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W))
			movement.z += 1;
		if (Input.GetKey(KeyCode.S))
			movement.z -= 1;
		if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
			movement.x -= 1;
		if (Input.GetKey(KeyCode.D))
			movement.x += 1;

		movement.Normalize();
		Vector3 RootForwards = TrackedValues.HmdRotation * Vector3.forward;
		RootForwards.y = 0;
		RootForwards = RootForwards.normalized;
		Quaternion rootRotation = Quaternion.LookRotation(RootForwards);
		Vector3 displacement = rootRotation * movement * 3.5f * Time.deltaTime;

		transform.position += displacement;
	}
}
