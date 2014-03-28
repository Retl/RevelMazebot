using UnityEngine;
using System.Collections;

public class CarBehavior : MonoBehaviour {

	public float moveSpeed = 10.0f;
	public float timeSinceChange = 0.0f;
	public enum State {GoForward, TurnLeft, TurnRight, TurnAround, BackUp}

	public State state;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timeSinceChange += Time.deltaTime;

		ForwardPulse();

		switch (state) 
		{
		case State.GoForward:
			ForwardPulse();
			break;

		case State.BackUp:
			BackwardPulse();
			break;
		default: 
			ForwardPulse();
			break;
		}
	
	}

	public void ForwardPulse()
	{
		rigidbody.velocity = moveSpeed * transform.forward * Time.deltaTime;
	}

	public void BackwardPulse()
	{
		rigidbody.velocity = -moveSpeed * transform.forward * Time.deltaTime;
	}

	public void TurnLeft()
	{
		transform.Rotate( new Vector3(0.0f, -90.0f, 0.0f));
	}

	public void TurnRight()
	{
		transform.Rotate( Vector3.right * 90);
		print ("Turning Right");
	}
}
