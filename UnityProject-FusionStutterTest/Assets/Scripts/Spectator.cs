using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(999)]
public class Spectator : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	private int _index = -1;
	private Transform _followTarget;
	// Update is called once per frame
	void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			_index = ++_index % NetPlayer.Players.Count;
			var player = NetPlayer.Players[_index];
			_followTarget = player.HeadTransform.InterpolationTarget;
		}

		if (_followTarget != null)
		{
			transform.SetPositionAndRotation(_followTarget.position, _followTarget.rotation);
		}
	}
}
