using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other){
        /*if (other.CompareTag("Player")){
			SPZController PlayerCtrl = other.GetComponent<SPZController>();
			SpecialEffectsHelper.Instance.GetEaten(PlayerCtrl.mouth.transform.position);
			Destroy(gameObject);
             Debug.Log("ouep "+ LayerMask.LayerToName(other.gameObject.layer));
		}*/

    }
}
