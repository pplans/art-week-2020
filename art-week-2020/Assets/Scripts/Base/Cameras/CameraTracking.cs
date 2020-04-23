using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
	public Transform target;

	public void Update()
	{
		transform.position = Vector3.Lerp(transform.position, target.position, 0.1f);
	}
}
