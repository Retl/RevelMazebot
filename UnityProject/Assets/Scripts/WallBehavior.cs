using UnityEngine;
using System.Collections;

public class WallBehavior : MonoBehaviour
{

    void OnCollisionEnter(Collision other)
    {
        print("Wall Touching.");
        //other.gameObject.SendMessage("OnCollisionEnter", other, SendMessageOptions.DontRequireReceiver);
        LeftWhiskerBehavior lwb = transform.parent.gameObject.GetComponent<LeftWhiskerBehavior>();
        RightWhiskerBehavior rwb = transform.parent.gameObject.GetComponent<RightWhiskerBehavior>();

        if (lwb != null)
        {
            lwb.Collided(other);
        }

        if (rwb != null)
        {
            rwb.Collided(other);
        }

    }
}
