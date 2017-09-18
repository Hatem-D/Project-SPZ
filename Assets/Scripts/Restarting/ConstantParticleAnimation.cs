using UnityEngine;
using System.Collections;

public class ConstantParticleAnimation : MonoBehaviour {

	float emitSpeed, emitRate;
    GameSpeedManager sm;
    ParticleSystem ps;

	// Use this for initialization
	void Start () {
        sm = GameObject.FindGameObjectWithTag(GlobalTags.GameManager).GetComponent<GameSpeedManager>();
        ps = GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
		emitSpeed = Mathf.Lerp(emitSpeed,sm.GameSpeed * 3.0f, Time.deltaTime/2.0f);
		ps.startSpeed = emitSpeed;
	}
}