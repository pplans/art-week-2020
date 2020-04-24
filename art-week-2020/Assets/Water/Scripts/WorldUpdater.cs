using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldUpdater : MonoBehaviour {

    public Water sea;

    public float seaWidth = 12.0f; // supposed to be 10 but we had a margin


    class BezierCurve
	{
		public Vector3 P1;
		public Vector3 P2;
		public Vector3 P3;
		public Vector3 P4;
		public Vector3 Get2(float t)
		{
			return (1.0f - t) * (1.0f - t) * P1 + 2.0f * (1.0f - t) * t * P2 + t * t * P3;
		}
		public Vector3 Get3(float t)
		{
			return (1.0f - t) * (1.0f - t) * (1.0f -t) * P1
			+ 3.0f * (1.0f - t) * (1.0f - t) * t * P2
			+ 3.0f * (1.0f - t) * (1.0f - t) * t * P3
			+ t * t * t * P4;
		}
	}

    // Use this for initialization
    void Start ()
	{
    }

	public void Reset ()
	{
		sea.Offset = Vector2.zero;

        Generate();
	}


	void Generate()
	{
    }

    // Update is called once per frame
    void Update ()
	{
	}
}
