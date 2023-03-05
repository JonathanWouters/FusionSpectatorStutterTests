using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class NetPlayer : NetworkBehaviour
{
	public NetworkTransform RootTransform;
	public NetworkTransform HeadTransform;
	public NetworkTransform LeftHandTransform;
	public NetworkTransform RightHandTransform;

	public static List<NetPlayer> Players = new List<NetPlayer>();
	public override void Spawned()
	{
		if (Object.HasInputAuthority)
		{
			RootTransform.InterpolationDataSource = InterpolationDataSources.NoInterpolation;
			HeadTransform.InterpolationDataSource = InterpolationDataSources.NoInterpolation;
			LeftHandTransform.InterpolationDataSource = InterpolationDataSources.NoInterpolation;
			RightHandTransform.InterpolationDataSource = InterpolationDataSources.NoInterpolation;
		}

		Players.Add(this);
	}

	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		Players.Remove(this);
	}

	public override void FixedUpdateNetwork()
	{
		if (GetInput(out PlayerInput input))
		{
			RootTransform.Transform.SetPositionAndRotation(input.RootPosition, input.RootRotation);

			HeadTransform.Transform.localPosition = input.HeadPosition;
			HeadTransform.Transform.localRotation = input.HeadRotation;

			LeftHandTransform.Transform.localPosition = input.LeftHandPosition;
			LeftHandTransform.Transform.localRotation = input.LeftHandRotation;

			RightHandTransform.Transform.localPosition = input.RightHandPosition;
			RightHandTransform.Transform.localRotation = input.RightHandRotation;
		}
	}

	public void LateUpdate()
	{
		if (Object !=null && Object.IsValid && Object.HasInputAuthority)
		{
			Vector3 RootPosition = CameraRig.Instance.Hmd.position;
			RootPosition.y = 0;

			Vector3 RootForwards = CameraRig.Instance.Hmd.rotation * Vector3.forward;
			RootForwards.y = 0;
			RootForwards = RootForwards.normalized;
			RootTransform.Transform.SetPositionAndRotation(RootPosition, Quaternion.identity);
			
			HeadTransform.Transform.localPosition = CameraRig.Instance.Hmd.localPosition;
			HeadTransform.Transform.localRotation = CameraRig.Instance.Hmd.localRotation;

			LeftHandTransform.Transform.localPosition = CameraRig.Instance.LeftHand.localPosition;
			LeftHandTransform.Transform.localRotation = CameraRig.Instance.LeftHand.localRotation;

			RightHandTransform.Transform.localPosition = CameraRig.Instance.RightHand.localPosition;
			RightHandTransform.Transform.localRotation = CameraRig.Instance.RightHand.localRotation;
		}
	}
}

public struct PlayerInput: INetworkInput
{
	public Vector3    RootPosition;
	public Quaternion RootRotation;

	public Vector3    HeadPosition;
	public Quaternion HeadRotation;

	public Vector3    LeftHandPosition;
	public Quaternion LeftHandRotation;

	public Vector3    RightHandPosition;
	public Quaternion RightHandRotation;
}