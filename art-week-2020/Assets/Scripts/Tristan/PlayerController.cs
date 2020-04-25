using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int score = 0;
    public float speed  = 10.0F;
    public float rotationSpeed = 100.0F;

    public float horizontal;
    public float vertical;

    private Rigidbody rb;
    Vector3 EulerAngleVelocity = new Vector3(0,100,0);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (vertical != 0)
            {
                rb.MovePosition(transform.position + transform.forward * speed * vertical * Time.deltaTime);
            }

        if (horizontal != 0)
            {
                Quaternion deltaRotation = Quaternion.Euler(EulerAngleVelocity * horizontal * Time.deltaTime);
                rb.MoveRotation(rb.rotation * deltaRotation);
            }
    }
}
