using UnityEngine;
using System.Collections;

public class TimedObjectDestroyer : MonoBehaviour
{
	//per far sparire i colpi dal scene
	public float lifeTime = 10.0f;

	// Use this for initialization
	void Start()
	{
		Destroy(gameObject, lifeTime);
	}
}