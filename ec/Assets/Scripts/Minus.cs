using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	public class Minus : NetworkBehaviour
	{
		public static float MINUS_BORDER_SIZE = 3f;
		public bool IsGameStarted = false;
		public GameObject Player1;
		public GameObject Player2;
		public float Speed = -1f;
		public GameObject Plate;
		private Vector3 _plateCenter;
		private float _halfSizeMinus;
		private bool _arePlayersSet = false;

		private bool _isInCenter = false;
		private float _centeredSeconds = 0;
		public bool IsWin = false;

		private GameObject _arrowObject;
		private const float _originArrowYScale = 0.5f;

		private float _plateRadius;
		void Start () {
			if (Plate == null) Plate = GameObject.Find("Plate");
			_plateCenter = Plate.GetComponent<MeshRenderer>().bounds.center;
			_halfSizeMinus = this.transform.GetComponentInChildren<SphereCollider>().bounds.size.x / 2;
			_plateRadius = (Plate.transform.GetComponent<MeshCollider>().bounds.size.x / 2) - _halfSizeMinus;
			IsWin = false;
			_arrowObject = this.transform.GetChild(0).gameObject;
		}

		void Update ()
		{
			if (!isServer)
			{
				return;
			}
			if (!IsGameStarted || IsWin)
			{
				return;
			}
			if (!_arePlayersSet)
			{
				SetPlayers();
			}

			Vector3 nextPosFromPlayer1 = CalculateNextPositionReletiveTo(Player1);
			Vector3 nextPosFromPlayer2 = CalculateNextPositionReletiveTo(Player2);
			Vector3 nextPos = Vector3.Lerp(nextPosFromPlayer1, nextPosFromPlayer2, 0.5f);
			UpdateArrow(nextPos);
			if (Vector3.Distance(_plateCenter, nextPos) < MINUS_BORDER_SIZE)
			{
				Vector3 direction = (nextPos - transform.position).normalized;
				Quaternion lookRotation = Quaternion.LookRotation(direction);
				transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, float.MaxValue - 1);

				this.transform.position = nextPos;
			}
			CheckWinCondition();
		}

		Vector3 CalculateNextPositionReletiveTo(GameObject player)
		{
			float distanceBetweenPlayerAndMinus = Vector3.Distance(this.transform.position, player.transform.position);
			float playerAffectSpeed = Speed / distanceBetweenPlayerAndMinus;
			float step = playerAffectSpeed * Time.deltaTime;
			Vector3 pointToMoveFor = Vector3.Lerp(player.transform.position, player.transform.position, 0.5f);

			return Vector3.MoveTowards(this.transform.position, pointToMoveFor, step);
		}

		void SetPlayers()
		{
			GameObject[] playersByTag = GameObject.FindGameObjectsWithTag("Player");
			Player1 = playersByTag[0];
			Player2 = playersByTag[1];
			_arePlayersSet = true;
		}

		void CheckWinCondition()
		{
			Debug.Log("_centeredSeconds = " + _centeredSeconds + "  (" + 500 + ")");
			if (CheckIsInCenter() && IsGameStarted)
			{
				if (_centeredSeconds == 0 && !_isInCenter)
				{
					StartCoroutine(WaitUntilBalanced());
				}
				_centeredSeconds++;
			}
			else
			{
				_centeredSeconds = 0;
			}
		}

		IEnumerator WaitUntilBalanced()
		{
			_isInCenter = true;
			yield return new WaitUntil(() => _centeredSeconds >= 500 );
			StopCoroutine(WaitUntilBalanced());
			_isInCenter = false;
			IsWin = true;
		}

		bool CheckIsInCenter()
		{
			Debug.Log("Distance from center = " + Vector3.Distance(_plateCenter, this.transform.position));
			return Vector3.Distance(_plateCenter, this.transform.position) < 1f;
		}

		void UpdateArrow(Vector3 nextPos)
		{
			float dPosition =  this.transform.position.magnitude / nextPos.magnitude;
			_arrowObject.transform.localScale = new Vector3(dPosition, _arrowObject.transform.localScale.y,
				_arrowObject.transform.localScale.z);

			PositioningArrow();
			Debug.Log("dPosition =  " + dPosition);
		}

		void PositioningArrow()
		{
			//_arrowObject.transform.localPosition = Vector3.zero;
			_arrowObject.transform.localPosition = new Vector3(0, 0, (_arrowObject.GetComponent<MeshRenderer>().bounds.extents.z));
		}
	}
}
