using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieGimble : MonoBehaviour
{
	private Transform[] faces = new Transform[6];
	private void Awake()
	{
		Transform[] trans = GetComponentsInChildren<Transform>();
		for (int i = 1; i < trans.Length; i++)
		{
			faces[i - 1] = trans[i];
		}
	}
	public int GetTopFaceIndex()
	{
		for (int i = 0; i < faces.Length; i++)
		{
			if (faces[i].transform.position - transform.position == Vector3.up / 2)
			{
				Debug.Log(i);
				return i;
			}
		}
		Debug.Log(-1);
		return -1;

	}
}
