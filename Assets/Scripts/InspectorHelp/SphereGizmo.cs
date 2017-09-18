using UnityEngine;
using System.Collections;

public class SphereGizmo : MonoBehaviour {

    public Color gColor;

	void OnDrawGizmos()
    {
        Gizmos.color = gColor;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
