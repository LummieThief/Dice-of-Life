using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
	[SerializeField] GameObject model;
	[SerializeField] CapsuleCollider collidr;
	[SerializeField] ParticleSystem particles;
	public static int numStars;
	private bool collected;

	public void Collect()
	{
		if (!collected)
		{
			collected = true;
			model.SetActive(false);
			collidr.enabled = false;
			numStars--;
			if (numStars == 0)
			{
				AudioController.instance.GetSound(3).Play();
				MainCanvas.instance.OnWin();
			}
			else
			{
				AudioController.instance.GetSound(2).Play();
			}
			particles.Play();
		}
	}

	public void UnCollect()
	{
		if (collected)
		{
			collected = false;
			model.SetActive(true);
			collidr.enabled = true;
			numStars++;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Collect();
	}
}
