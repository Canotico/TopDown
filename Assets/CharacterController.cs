using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed = 10;
    Rigidbody2D myRigidbody;

	// Use this for initialization
	void Start ()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector2 newPosition = myRigidbody.position;
        if (Input.GetKey(KeyCode.UpArrow))
            newPosition += Vector2.up * Time.deltaTime * speed;
        else if (Input.GetKey(KeyCode.DownArrow))
            newPosition += Vector2.down * Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.LeftArrow))
            newPosition += Vector2.left * Time.deltaTime * speed;
        else if (Input.GetKey(KeyCode.RightArrow))
            newPosition += Vector2.right * Time.deltaTime * speed;

        myRigidbody.MovePosition(newPosition);
    }
}
