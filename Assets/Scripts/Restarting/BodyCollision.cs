using UnityEngine;
using System.Collections;

public class BodyCollision : MonoBehaviour {

	SPZController mySPZ;


	// Use this for initialization
	void Start () {
		mySPZ = GetComponentInParent<SPZController> ();
		if (mySPZ == null)
			Debug.Log ("warning no spzcontroller for "+gameObject.name);		
	}
	


	void OnTriggerEnter2D(Collider2D collider){
		//Debug.Log (collider.gameObject.name);
		if (LayerMask.Equals (collider.gameObject.layer, this.gameObject.layer)) {
			mySPZ.collidedGameObject = collider.gameObject;
			mySPZ.bodyCollision ();
		}
	}
}
