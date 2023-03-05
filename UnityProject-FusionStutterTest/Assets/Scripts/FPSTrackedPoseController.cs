using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class FPSTrackedPoseController : TrackedPoseController
{
	public float LookSensitivity = 2;

	public Vector3 HeadPosition = new Vector3(0, 1.7f, 0);

	public Vector3 LeftHandPosition;
	public Vector3 LeftHandRotation;

	public Vector3 RightHandPosition;
	public Vector3 RightHandRotation;

	float _horizontalCameraAngle = 0;
	float _verticalCameraAngle = 0;

	private TrackedValues _previousValues;
	Vector2 _mouseDelta;
	private bool _isFocused;

	public void OnValidate()
	{
		bool temp = _isFocused;
		_isFocused = true;
		_previousValues = GetTrackedValues();
		_isFocused = temp;
	}

	public override TrackedValues GetTrackedValues()
	{

		if (_isFocused == false)
		{
			_mouseDelta = Vector2.zero;
			return _previousValues;
		}

		Vector2 mouseDelta = LookSensitivity * _mouseDelta;
		_mouseDelta = Vector2.zero;

		_horizontalCameraAngle += mouseDelta.x;
		_verticalCameraAngle -= mouseDelta.y;
		_horizontalCameraAngle %= 360;
		_verticalCameraAngle %= 360;

		_verticalCameraAngle = Mathf.Clamp(_verticalCameraAngle, -80, 80);

		Quaternion horizontal = Quaternion.AngleAxis(_horizontalCameraAngle, Vector3.up);
		Quaternion vertical = Quaternion.AngleAxis(_verticalCameraAngle, Vector3.right);

		TrackedValues trackedValues = new TrackedValues();

		trackedValues.HmdPosition = HeadPosition;
		trackedValues.HmdRotation = horizontal * vertical;

		Vector3 handoffset = trackedValues.HmdPosition;
		handoffset.y = 0;
		trackedValues.LeftControllerPosition = RotatePointAroundPivot(LeftHandPosition + handoffset, trackedValues.HmdPosition, trackedValues.HmdRotation);
		trackedValues.LeftControllerRotation = trackedValues.HmdRotation * Quaternion.Euler(LeftHandRotation);

		trackedValues.RightControllerPosition = RotatePointAroundPivot(RightHandPosition + handoffset, trackedValues.HmdPosition, trackedValues.HmdRotation);
		trackedValues.RightControllerRotation = trackedValues.HmdRotation * Quaternion.Euler(RightHandRotation);

		_previousValues = trackedValues;
		return trackedValues;
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
	{
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = rotation * dir; // rotate it
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}

	public void Update()
	{
		_mouseDelta.x += Input.GetAxis("Mouse X");
		_mouseDelta.y += Input.GetAxis("Mouse Y");

		if (_isFocused)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				_isFocused = false;
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}
		else
		{
			if (Input.GetMouseButtonDown(0) && Camera.main != null)
			{
				var view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
				var isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
				if (isOutside == false && IsPointerOverUIElement() == false)
				{
					_isFocused = true;
					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;
				}
			}
		}

		bool holdingCtrl = Input.GetKey(KeyCode.LeftControl);
		if (holdingCtrl)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	public static bool IsPointerOverUIElement()
	{
		if (EventSystem.current == null) return false;
		var eventData = new PointerEventData(EventSystem.current);
		eventData.position = UnityEngine.Input.mousePosition;
		var results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, results);
		return results.Count > 0;
	}
}

