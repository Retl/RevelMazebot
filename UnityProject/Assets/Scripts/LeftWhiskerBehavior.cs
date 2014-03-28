using UnityEngine;
using System.Collections;

public class LeftWhiskerBehavior : MonoBehaviour {

    public delegate void PublicDelegate(Collision other);
    public PublicDelegate Collided;

	// Use this for initialization
	void Start () {
        Collided = OnCollisionEnter;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCollisionEnter( Collision me)
	{
		CarBehavior cbs = transform.parent.gameObject.GetComponent<CarBehavior>();
		print ("Left Whisker Touching.");
		//SendMessageUpwards("TurnRight", true, SendMessageOptions.DontRequireReceiver);

		if (cbs != null) {
			cbs.TurnRight();
		}
	}
}
