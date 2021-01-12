using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : NetworkManager
{
	public List<Player> m_players = new List<Player>();

	public GameObject m_timePanel;

	public GameObject m_networkButtonsPanel;
	public GameObject menuBackground;
	public Button m_exitButton;
	public GameObject m_mobileController;
	public GameObject MinusObject;
	public GameObject m_gameResultsPanel;
	public GameObject m_chargeControllPanel;
	public GameObject m_waitingForSecondPlayerPanel;

	public Timer m_timer;

	private float _halfSizePlayer;
	private float _plateRadius;
	private List<Vector3> _posList;
	public GameObject Plate;

	private GameObject localPlayer;
	private Vector3 plateCenter;

	#region
	public static GameController Instance { get; private set; }
	#endregion

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		m_waitingForSecondPlayerPanel.SetActive(true);
		int playersCount = NetworkServer.connections.Count;
		if (playersCount <= 2)
		{
			localPlayer = Instantiate(
				playerPrefab,
				playerPrefab.transform.position,
				Quaternion.identity);

			NetworkServer.AddPlayerForConnection(conn, localPlayer, playerControllerId);

			_halfSizePlayer = localPlayer.transform.GetComponentInChildren<SphereCollider>().bounds.size.x / 2;

			_plateRadius = (Plate.transform.GetComponent<MeshCollider>().bounds.size.x / 2) - _halfSizePlayer * 2;
			localPlayer.GetComponent<Player>().Plate = Plate;
			
			m_players.Add(localPlayer.GetComponent<Player>());
			localPlayer.transform.position = GeneratePosition();

			if (playersCount == 2)
			{
				m_waitingForSecondPlayerPanel.SetActive(false);
				MinusObject.GetComponent<Minus>().IsGameStarted = true;
				ReneratePostitionOfPLayers();
				m_players[0].RpcShowTime();
				m_players[1].CmdShowTime();

				m_players[1].RpcChangeMaterialOnYellow();
				m_players[1].CmdChangeMaterialOnYellow();

				m_players[0].Name = "Player " + 1;
				m_players[1].Name = "Player " + 2;
			}
		}
		else
		{
			conn.Disconnect();
		}
	}

	public override void OnStopHost()
	{
		Debug.Log("Host stop");
		NetworkServer.DisconnectAll();
	}

	public override void OnStopClient()
	{
		Debug.Log("Client stop");
		base.OnStopClient();
	}

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		Debug.Log("OnClientDisconnect");
		base.OnClientDisconnect(conn);
		conn.Disconnect();
	}

	private void Awake()
	{
		Instance = this;
		m_timePanel.SetActive(false);
		m_gameResultsPanel.SetActive(false);
		m_exitButton.gameObject.SetActive(false);
		m_chargeControllPanel.SetActive(false);
		m_mobileController.SetActive(false);
	}

	void Start()
	{
		plateCenter = Plate.GetComponent<MeshRenderer>().bounds.center;
		m_gameResultsPanel.SetActive(false);
	}

	// custom network hud control
	public UnityEngine.UI.Text hostNameInput;

	public void StartLocalGame()
	{
		StartHost();
		m_networkButtonsPanel.SetActive(false);
		m_gameResultsPanel.SetActive(false);
		menuBackground.SetActive(false);
		m_timePanel.SetActive(true);
		m_mobileController.SetActive(true);
		m_exitButton.gameObject.SetActive(true);
		m_chargeControllPanel.SetActive(true);
	}

	public void JoinLocalGame()
	{
		if (hostNameInput.text != "Enter your IP...")
		{
			networkAddress = hostNameInput.text;
		}
		StartClient();
		m_networkButtonsPanel.SetActive(false);
		m_gameResultsPanel.SetActive(false);
		menuBackground.SetActive(false);
		m_mobileController.SetActive(true);
		m_exitButton.gameObject.SetActive(true);
		m_chargeControllPanel.SetActive(true);
	}

	public void ExitGame()
	{
		if (NetworkServer.active)
		{
			StopServer();
		}
		if (NetworkClient.active)
		{
			StopClient();
		}
		m_players.Clear();
		m_networkButtonsPanel.SetActive(true);
		menuBackground.SetActive(true);
		m_mobileController.SetActive(false);
		m_exitButton.gameObject.SetActive(false);
		m_chargeControllPanel.SetActive(false);
		Application.Quit();
	}

	void ReneratePostitionOfPLayers()
	{
		_posList = new List<Vector3>();
		Vector3 player1Pos = GeneratePosition();
		_posList.Add(player1Pos);
		localPlayer.transform.position = player1Pos;
	}

	Vector3 GeneratePosition()
	{
		var nextPos = new Vector3(Random.Range(-_plateRadius, _plateRadius), playerPrefab.transform.position.y, Random.Range(-_plateRadius, _plateRadius));

		while (Vector3.Distance(plateCenter, nextPos) <= 6f)
		{
			nextPos = new Vector3(Random.Range(-_plateRadius, _plateRadius), playerPrefab.transform.position.y, Random.Range(-_plateRadius, _plateRadius));
		}

		return nextPos;
	}

	Vector3 GeneratePositionExcludeListPos(List<Vector3> excludeList)
	{
		Vector3 playerPos = GeneratePosition();
		excludeList.ForEach(a =>
		{
			while (Vector3.Distance(a, playerPos) <= _halfSizePlayer)
			{
				playerPos = GeneratePosition();
			}
		});

		return playerPos;
	}

	private void SetRndomMass(GameObject obj)
	{
		float masstoset = Random.Range(0.5f, 1);
		obj.GetComponent<Rigidbody>().mass = masstoset;
	}

	public void PlayClick()
	{
		AudioSource audioSource = GetComponent<AudioSource>();
		audioSource.Play();
	}
}
