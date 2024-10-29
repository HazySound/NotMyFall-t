using UnityEngine;
using System.Collections;

public class laserScript : MonoBehaviour {
	public Transform startPoint;
	public Transform endPoint;
	public LineRenderer laserLine;
	// Use this for initialization
	void Start () {
		//laserLine = GetComponentInChildren<LineRenderer> ();
		//laserLine.SetWidth (.2f, .2f);
		laserLine.startWidth = 0.2f;
		laserLine.endWidth = 0.2f;
	}
	
	// Update is called once per frame
	void Update () {
		laserLine.SetPosition (0, startPoint.position);
		laserLine.SetPosition (1, endPoint.position);

	}
}
