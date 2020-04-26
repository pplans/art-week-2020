using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canonball : MonoBehaviour
{
	[SerializeField]
	public float _speed;

	private Rigidbody rb;
	// Start is called before the first frame update
	void Start()
    {
		rb = GetComponent<Rigidbody>();
		Object.Destroy(transform.gameObject, 3);
    }

    // Update is called once per frame
    void Update()
    {
		rb.MovePosition(transform.position + transform.forward * _speed * Time.deltaTime);
	}

	void OnCollisionEnter()
	{
		Object.Destroy(transform.gameObject);
	}
}
