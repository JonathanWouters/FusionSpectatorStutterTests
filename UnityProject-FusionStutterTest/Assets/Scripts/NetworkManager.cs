using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
	[SerializeField] private NetworkPrefabRef _playerPrefab;

	[SerializeField] private GameObject _playerCamera;
	[SerializeField] private GameObject _spectatorCamera;
	[SerializeField] private GameObject _menuCamera;

	private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

	public void Awake()
	{
		_playerCamera.SetActive(false);
		_spectatorCamera.SetActive(false);
		_menuCamera.SetActive(true);
	}

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		byte[] bytes = runner.GetPlayerConnectionToken(player);
		if (bytes != null)
		{
			if (bytes[0] == 1)
			{
				// Create a unique position for the player
				Vector3 spawnPosition = Vector3.zero;
				NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
				// Keep track of the player avatars so we can remove it when they disconnect
				_spawnedCharacters.Add(player, networkPlayerObject);
			}

			if (bytes[0] == 2)
			{

			}
		}

	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		// Find and remove the players avatar
		if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
		{
			runner.Despawn(networkObject);
			_spawnedCharacters.Remove(player);
		}
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		Vector3 RootPosition = CameraRig.Instance.Hmd.position;
		RootPosition.y = 0;

		Vector3 RootForwards = CameraRig.Instance.Hmd.rotation * Vector3.forward;
		RootForwards.y = 0;
		RootForwards = RootForwards.normalized;
		Quaternion rootRotation = Quaternion.LookRotation(RootForwards);
		


		var data = new PlayerInput
		{
			RootPosition = RootPosition,
			RootRotation = Quaternion.identity,

			HeadPosition = CameraRig.Instance.Hmd.localPosition,
			HeadRotation = CameraRig.Instance.Hmd.localRotation,

			LeftHandPosition = CameraRig.Instance.LeftHand.localPosition,
			LeftHandRotation = CameraRig.Instance.LeftHand.localRotation,

			RightHandPosition = CameraRig.Instance.RightHand.localPosition,
			RightHandRotation = CameraRig.Instance.RightHand.localRotation,
		};

		input.Set(data);
	}

	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		_playerCamera.SetActive(false);
		_spectatorCamera.SetActive(false);
		_menuCamera.SetActive(true);
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
	public void OnSceneLoadDone(NetworkRunner runner) 
	{


		//Vector3 spawnPosition = Vector3.zero;
		//if (_spawnBots)
		//{
		//	runner.Spawn(_targetPrefab, spawnPosition, Quaternion.identity);
		//	runner.Spawn(_targetPrefab, spawnPosition, Quaternion.identity);
		//	runner.Spawn(_targetPrefab, spawnPosition, Quaternion.identity);
		//	runner.Spawn(_targetPrefab, spawnPosition, Quaternion.identity);
		//	runner.Spawn(_targetPrefab, spawnPosition, Quaternion.identity);
		//}
	}

	public void OnSceneLoadStart(NetworkRunner runner) { }

	async void StartGame(GameMode mode, bool spectator = false)
	{
		// Create the Fusion runner and let it know that we will be providing user input
		_runner = gameObject.AddComponent<NetworkRunner>();
		_runner.ProvideInput = true;

		byte[] data = { spectator ? (byte)2 : (byte)1 };

		// Start or join (depends on gamemode) a session with a specific name
		await _runner.StartGame(new StartGameArgs()
		{
			GameMode = mode,
			SessionName = "TestRoom",
			Scene = SceneManager.GetActiveScene().buildIndex,
			SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
			ConnectionToken = data
		});
	}

	private NetworkRunner _runner;

	private void OnGUI()
	{

		if (_runner == null)
		{
			if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
			{
				_menuCamera.SetActive(false);
				_playerCamera.SetActive(true);
				StartGame(GameMode.Host);
			}
			if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
			{
				_menuCamera.SetActive(false);
				_playerCamera.SetActive(true);
				StartGame(GameMode.Client);
			}
			if (GUI.Button(new Rect(0, 80, 200, 40), "Join spectator"))
			{
				_menuCamera.SetActive(false);
				_spectatorCamera.SetActive(true);
				StartGame(GameMode.Client, true);
			}
		}
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{

	}

	public void OnConnectedToServer(NetworkRunner runner)
	{

	}
}
