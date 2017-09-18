using UnityEngine;
using System.Collections;

public class LightParticleBehaviour : MonoBehaviour {

    Light myLight;

    public bool lerpRange = true;
    public float minRange, maxRange, varianceSpeed;
    float targetRange;

    delegate void doLightStuffFunctionPointer();
    doLightStuffFunctionPointer doLightStuff;

    // Use this for initialization
    void Start () {
        myLight = gameObject.GetComponent<Light>();
        targetRange = minRange;

        if (lerpRange) doLightStuff += LerpRange;
	}
	
	// Update is called once per frame
	void Update () {

        if (doLightStuff != null)
        {
            doLightStuff();
        }
    }

    void LerpRange()
    {
        myLight.range = Mathf.Lerp(myLight.range, targetRange, varianceSpeed * Time.deltaTime);

        if ((Mathf.Abs(myLight.range - maxRange) < 0.1f) || (myLight.range > maxRange)) { targetRange = minRange; }
        if ((Mathf.Abs(myLight.range - minRange) < 0.1f) || (myLight.range < minRange)) { targetRange = maxRange; }

    }

}
