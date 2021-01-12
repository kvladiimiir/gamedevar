using System;
using System.Collections;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class Player : NetworkBehaviour
{
	public Material YellowLadyBug;
	public Material PurpleLadyBug;
	private bool isMassSet = false;
	float gravity = 15;

	[SyncVar]
	float m_seconds = 0;

	public GameObject Plate;
	public float PLayerSpeed = 4f;

	[SyncVar]
	public String Name;
	private Vector3 plateCenter;
	private float plateRadius;
	private float halfSizePlayer;

	private float winTime = 0;

	[Command]
	public void CmdLog(int playersCount)
	{
		Debug.Log("Created at playercCount = " + playersCount);
	}

	[ClientRpc]
	public void RpcLog(int playersCount)
	{
		Debug.Log("Created at playercCount = " + playersCount);
	}

	void Start()
	{
		if(Plate == null) Plate = GameObject.Find("Plate");
		plateCenter = Plate.GetComponent<MeshRenderer>().bounds.center;
		halfSizePlayer = this.transform.GetComponentInChildren<SphereCollider>().bounds.size.x / 2;
		plateRadius = (Plate.transform.GetComponent<MeshCollider>().bounds.size.x / 2) - halfSizePlayer;
		//FreezePozition();
	}

	[ClientCallback]
	void Update()
	{
		SetName();
		transform.GetChild(0).GetChild(0).transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
		transform.GetChild(0).transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
		if (!isLocalPlayer)
		{
			return;
		}
		if (!hasAuthority)
		{
			return;
		}
		if (isServer && GameController.Instance.m_players.Count == 2)
		{
			m_seconds += Time.deltaTime;
			RpcUpdateTime();
		}
		else if (!isServer)
		{
			m_seconds = GameController.Instance.m_timer.GetTime();
			CmdUpdateTime();
		}

		if (GameController.Instance.MinusObject.GetComponent<Minus>().IsWin && winTime == 0)
		{
			winTime = (float)Math.Round(GameController.Instance.m_timer.GetTime(), 2);
			RpcShowGameResultsText("Win!\n It took: " + winTime + " s");
		}
		else
		{
			//UnfreezePozition();
			MovePLayer();
		}
	}

	void FreezePozition()
	{
		this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
	}

	void UnfreezePozition()
	{
		this.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionX | ~RigidbodyConstraints.FreezePositionZ;
	}
	void MovePLayer()
	{
		float x = (Input.GetAxis("Horizontal") + CrossPlatformInputManager.GetAxis("Horizontal")) * 0.5f;
		float z = (Input.GetAxis("Vertical") + CrossPlatformInputManager.GetAxis("Vertical")) * 0.5f;

		/*if (Math.Abs(x) < 0 && Math.Abs(z) < 0)
		{
			FreezePozition();
		}*/

		Vector3 movement = new Vector3(x, 0, z);
		Vector3 nextPos = this.transform.position + movement * Time.deltaTime * PLayerSpeed;

		if (x != 0 && z != 0)
		{
			this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, Mathf.Atan2(x, z) * Mathf.Rad2Deg,
				this.transform.eulerAngles.z);
		}

		if (Vector3.Distance(plateCenter, nextPos) < plateRadius && Vector3.Distance(plateCenter, nextPos) > 6f)
		{
			this.transform.position = nextPos;
		}
	}


	private void LateUpdate()
	{
	}

	public IEnumerator WaitSetAnimationTrigger() {
		yield return new WaitForSeconds(1.5f);
		Debug.Log("Stop wait animation");
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name == "PlayerUnit(Clone)")
		{
			Physics.IgnoreCollision(collision.collider, gameObject.GetComponentInChildren<MeshCollider>());
		}
	}

	void DrawLine()
	{
		Vector3 centerPos = Plate.GetComponent<MeshRenderer>().bounds.center + new Vector3(0, Plate.transform.localScale.y, 0);
		var lr = gameObject.GetComponent<LineRenderer>();
		lr.SetPosition(0, centerPos + new Vector3(0, 0.1f, 0));
		lr.SetPosition(1, this.transform.position + new Vector3(0, 0.3f, 0));
	}

	double ClculateDistance()
	{
		var distance = Vector3.Distance(transform.position, Plate.GetComponent<MeshRenderer>().bounds.center + new Vector3(0, Plate.transform.localScale.y, 0));
		return Math.Round(distance * 10, 0);
	}
	
	void SetDistance()
	{
		//transform.GetChild(1).GetComponentInChildren<TextMesh>().text = mass.ToString() + "(" + ClculateDistance() + ")";
	}

	public void CmdChangeMaterialOnPurple()
	{
		this.GetComponentInChildren<MeshRenderer>().material = PurpleLadyBug;
	}

	public void RpcChangeMaterialOnPurple()
	{
		this.GetComponentInChildren<MeshRenderer>().material = PurpleLadyBug;
	}

	[Command]
	public void CmdChangeMaterialOnYellow()
	{
		this.GetComponentInChildren<MeshRenderer>().material = YellowLadyBug;
	}
	[ClientRpc]
	public void RpcChangeMaterialOnYellow()
	{
		this.GetComponentInChildren<MeshRenderer>().material = YellowLadyBug;
	}

	public void ChangeMaterialOnPurple()
	{
		this.GetComponentInChildren<MeshRenderer>().material = PurpleLadyBug;
	}

	private void SetRndomMass()
	{
		
		RpcSetMass();
	}

	public void RpcSetMass()
	{
		//this.GetComponent<Rigidbody>().mass = mass;
	}
	[Command]
	public void CmdUpdateTime()
	{
		if (!GameController.Instance.MinusObject.GetComponent<Minus>().IsWin)
		{
			GameController.Instance.m_timer.SetTime(m_seconds);
		}
	}

	[ClientRpc]
	public void RpcUpdateTime()
	{
		if (!GameController.Instance.MinusObject.GetComponent<Minus>().IsWin)
		{
			GameController.Instance.m_timer.SetTime(m_seconds);
		}
	}

	[ClientRpc]
	public void RpcShowTime()
	{
		GameController.Instance.m_timePanel.SetActive(true);
		GameController.Instance.m_timer.SetPaused(false);
	}

	[Command]
	public void CmdShowTime()
	{
		GameController.Instance.m_timePanel.SetActive(true);
		GameController.Instance.m_timer.SetPaused(false);
	}

	[ClientRpc]
	public void RpcShowGameResultsText(string text)
	{
		GameController.Instance.m_gameResultsPanel.GetComponentInChildren<Text>().text = text;
		GameController.Instance.m_gameResultsPanel.SetActive(true);
	}

	[Command]
	public void CmdShowGameResultsText(string text)
	{
		RpcShowGameResultsText(text);
	}

	public void SetName()
	{
		transform.GetChild(0).GetComponentInChildren<TextMesh>().text = Name;
	}
}
