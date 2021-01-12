using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Plate : MonoBehaviour
{
	private bool isBalancing = false;
	private float balancedSeconds = 0;
	public bool isWin = false;
	public bool isGameStarted = false;

	void Start () {
		isWin = false;
	}
	
	void Update ()
	{
		RetrigerConvex();
		CheckPlateAngle();
	}

	void CheckPlateAngle()
	{
		var plateRotation = this.transform.rotation.eulerAngles;
		if (CheckAngle(plateRotation.x) && CheckAngle(plateRotation.z) && isGameStarted)
		{
			if (balancedSeconds == 0 && !isBalancing)
			{
				StartCoroutine(WaitUntilBalanced());
			}
			balancedSeconds++;
		}
		else
		{
			balancedSeconds = 0;
		}
	}
	IEnumerator WaitUntilBalanced()
	{
		isBalancing = true;
		yield return new WaitUntil(() => balancedSeconds >= 3 * Time.deltaTime);
		StopCoroutine(WaitUntilBalanced());
		isBalancing = false;
		isWin = true;
	}
	bool CheckAngle(float angle)
	{
		angle = angle < 0 ? angle * -1 : angle;
		return angle < 4f && angle >= 0;
	}

	void RetrigerConvex()
	{
		this.GetComponent<MeshCollider>().convex = false;
		this.GetComponent<MeshCollider>().convex = true;
	}
}
