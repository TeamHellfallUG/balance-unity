using UnityEngine;

public class NoCollisionVelocity : MonoBehaviour {

    void OnCollisionEnter(Collision other)
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().Sleep();
    }

}
