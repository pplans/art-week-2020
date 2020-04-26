using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;

public class Canonball : MonoBehaviour
{
	[SerializeField]
	public float _speed = 50.0f;

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

	void OnTriggerEnter(Collider other)
	{
		WorldObject o = other.gameObject.GetComponent<WorldObject>() as WorldObject;

		if(o.IsIA()){

			WorldObject parent = transform.parent.GetComponent<WorldObject>() as WorldObject;
			if(parent.IsPlayer()){
				Player player = parent as Player;
				player.AddScore(10);
				Debug.Log(player.Score);
			}
		}

		Object.Destroy(transform.gameObject);
	}
}
