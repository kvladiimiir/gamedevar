using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
	public GameObject Plate;

	private Vector3 minPos;
	private Vector3 centerPos;

	private LineRenderer lr;
	private GameObject sphere1;
	private GameObject sphere2;
	
	void Start ()
	{
		centerPos = Plate.GetComponent<MeshRenderer>().bounds.center;

		Plate.GetComponent<MeshFilter>().mesh.MarkDynamic();
		lr = gameObject.GetComponent<LineRenderer>();
		lr.SetPosition(0, centerPos);
		lr.SetPosition(1, minPos);

		//sphere1.transform.SetParent(Plate.transform);
		sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere2.GetComponent<SphereCollider>().enabled = false;
		sphere2.transform.position = minPos;


		sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere2.GetComponent<SphereCollider>().enabled = false;
		sphere1.transform.SetParent(Plate.transform);
		sphere1.transform.position = centerPos;
		//sphere2.transform.SetParent(Plate.transform);
	}
	
	void Update ()
	{
		Vector3 minVertex = Vector3.positiveInfinity;
		Vector3[] vertices = Plate.GetComponent<MeshFilter>().mesh.vertices;
		foreach (Vector3 vertex in vertices)
		{
			if (vertex.y < minVertex.y)
			{
				minVertex = vertex;
			}
		}
		Debug.Log(minVertex);

		Destroy(sphere2);
		//sphere1.transform.SetParent(Plate.transform);
		sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere2.GetComponent<SphereCollider>().enabled = false;
		sphere2.transform.SetParent(Plate.transform);
		sphere2.transform.localPosition = minVertex;
		//Plate.GetComponent<MeshFilter>().mesh.Clear();
		Plate.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		//sphere2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		//Debug.Log("BUGS: " + minVertex);
		lr.SetPosition(0, centerPos);
		lr.SetPosition(1, minVertex);
	}
}
