using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedTiling : MonoBehaviour {
	public float tileScale = 4.0f;
	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().material.mainTextureScale = new Vector2(transform.localScale.x * tileScale, transform.localScale.z * tileScale);
	}
}
