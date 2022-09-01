using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Die : MonoBehaviour
{
	/*
     * Dice have 6 faces that determine how they move.
     * Each face can have a pip value of 1 - 6 and a direction to determine how the die will roll when that face lands up.
     * The pips can also have a suit which adds a special ability to the face
     */
	private const float jumpForce = 100f;
	private const float rollForce = 50f;
	private const float rotationAlignThreshold = 10f;
	private const float grassThreshold = 1f;
	private const float aboutZero = 0.001f;

	private const float fiveModifier = 0.05f;
	private const float sixModifier = 0.25f;
	//private const float rollPoint = 0.75f; // 0 applies force directly to the center, 1 applies force directly to edge
	private const float velocitySleepThreshold = 0.1f;
	private const int velocityQueueDepth = 5;
	private float lastVelocity = 0f;
	private float lastAngularVelocity = 0f;
	private bool sound1Played = true;
	private bool sound2Played = true;
	private Queue<float> velocityQueue;


	public Transform u, d, l, r, f, b;

	public Face[] faces = new Face[6]; //0 = up, 1 = down, 2 = left, 3 = right, 4 = front, 5 = back
	private Rigidbody rb;


	public bool rolling = false;
	public bool running = false;

	private bool rollFlag;

	private Vector3 startingPos;
	private Quaternion startingRot;

	private int currentFace = 0;

	private DieCustomizer customizer;

	private Star[] stars;

	[SerializeField] ParticleSystem dustStrong, dustWeak;

	public bool randomize;


	
	

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		startingPos = transform.position;
		startingRot = transform.rotation;
		customizer = GetComponent<DieCustomizer>();
		if (customizer == null)
		{
			PaintPips();
			TryRoll();
		}

		stars = FindObjectsOfType<Star>();
		Star.numStars = stars.Length;
	}


	private void FixedUpdate()
	{
		if (MainCanvas.instance.paused)
			return;
		if (rolling)
		{
			if (velocityQueue.Dequeue() < velocitySleepThreshold && rb.velocity.magnitude < velocitySleepThreshold)
			{
				ResolveRoll();
			}
			else
			{
				velocityQueue.Enqueue(rb.velocity.magnitude);
			}
			RaycastHit hit;
			if (!sound1Played && customizer != null && Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.x / 2 * 1.1f))
			{
				sound1Played = true;
				AudioController.instance.GetSound(1).PlayOneShot(AudioController.instance.GetSound(1).clip, 0.2f);
			}

			if (customizer != null && transform.position.y < -10)
			{
				MainCanvas.instance.OnFallOut();
			}
		}
		else if (rollFlag)
		{
			Roll();
		}
		lastVelocity = rb.velocity.magnitude;
		/*
		if (Mathf.Abs(lastVelocity) - Mathf.Abs(rb.velocity.magnitude) > grassThreshold)
		{
			AudioController.instance.GetSound(1).PlayOneShot(AudioController.instance.GetSound(1).clip);
		}
		lastVelocity = rb.velocity.magnitude;
		*/
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!sound2Played && customizer != null)
		{
			sound2Played = true;
			AudioController.instance.GetSound(1).PlayOneShot(AudioController.instance.GetSound(1).clip, Mathf.Clamp((lastVelocity - rb.velocity.magnitude) / grassThreshold, 0, 1.2f));
		}
	}


	//AudioController.instance.GetSound(1).PlayOneShot(AudioController.instance.GetSound(1).clip);

	public void TryRoll()
	{
		rollFlag = true;
		running = true;
		if (customizer != null)
		{
			customizer.ResetCurrentUIFace();
		}
	}

	private void RefreshSound()
	{
		sound1Played = false;
		sound2Played = false;
	}

	private void Roll()
	{
		
		rollFlag = false;
		rb.isKinematic = false;
		transform.localEulerAngles = AlignRotation(transform.localEulerAngles);
		Face topFace = GetTopFace();
		float strength = topFace.value;
		if(strength == 5)
		{
			strength += fiveModifier;
		}
		else if(strength == 6)
		{
			strength += sixModifier;
		}
		float dir = topFace.dir;
		Vector3 topFaceDirection = GetTopFaceDirection();
		Vector3 frontFaceDirection = GetFrontFaceDirection();

		Debug.DrawRay(transform.position, topFaceDirection * 5, Color.red, 2f);

		rb.AddExplosionForce(jumpForce * strength, transform.position - topFaceDirection - frontFaceDirection * 0.5f , transform.localScale.x);
		rb.AddTorque(Vector3.Cross(topFaceDirection, frontFaceDirection) * rollForce * strength, ForceMode.Impulse);

		if (strength > 3)
		{
			dustStrong.transform.rotation = Quaternion.identity;
			dustStrong.Play();
		}
		else
		{
			dustWeak.transform.rotation = Quaternion.identity;
			dustWeak.Play();
		}
		

		rolling = true;
		velocityQueue = new Queue<float>();
		for (int i = 0; i < velocityQueueDepth; i++)
		{
			velocityQueue.Enqueue(velocitySleepThreshold);
		}

		if (customizer != null)
		{
			AudioController.instance.GetSound(1).PlayOneShot(AudioController.instance.GetSound(1).clip, (strength / 6));
			Invoke("RefreshSound", 0.2f);
		}
		

	}

	private void ResolveRoll()
	{
		rolling = false;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		if (customizer != null)
		{
			if (customizer.uipps.Length > 0)
			{
				customizer.uipps[currentFace].SetActive(false);
				currentFace = GetTopFaceIndex();
				customizer.uipps[currentFace].SetActive(true);
			}
		}
		Invoke("Roll", 0.25f);
		//Face topFace = GetFace(Vector3.up);
		//BehaviorManager.instance.GetBehavior(topFace.pip).OnRolledUp();
	}

	public void Return()
	{
		if (customizer != null)
		{
			transform.position = startingPos;
			transform.rotation = startingRot;
			rb.isKinematic = true;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rolling = false;
			rollFlag = false;
			running = false;

			if (customizer.uipps.Length > 0)
			{
				customizer.uipps[currentFace].SetActive(false);
				currentFace = 0;
				customizer.uipps[currentFace].SetActive(true);
			}

			CancelInvoke();
			foreach (Star s in stars)
			{
				s.UnCollect();
			}

			MainCanvas.instance.OnFallIn();
		}
	}

	private Vector3 GetTopFaceDirection()
	{
		float max = -999999;
		Transform t = null;
		if (u.position.y > max) { max = u.position.y; t = u; }
		if (d.position.y > max) { max = d.position.y; t = d; }
		if (l.position.y > max) { max = l.position.y; t = l; }
		if (r.position.y > max) { max = r.position.y; t = r; }
		if (f.position.y > max) { max = f.position.y; t = f; }
		if (b.position.y > max) { max = b.position.y; t = b; }
		return t.position - transform.position;
	}

	private int GetTopFaceIndex()
	{
		float max = -999999;
		int i = 0;
		if (u.position.y > max) { max = u.position.y; i = 0; }
		if (d.position.y > max) { max = d.position.y; i = 1; }
		if (l.position.y > max) { max = l.position.y; i = 2; }
		if (r.position.y > max) { max = r.position.y; i = 3; }
		if (f.position.y > max) { max = f.position.y; i = 4; }
		if (b.position.y > max) { max = b.position.y; i = 5; }
		return i;
	}
	private Face GetTopFace()
	{
		return faces[GetTopFaceIndex()];
	}

	private int GetFrontFaceIndex()
	{
		int topFaceIndex = GetTopFaceIndex();
		Face topFace = faces[topFaceIndex];
		int dir = topFace.dir;

		int frontFaceIndex = 0;

		switch(topFaceIndex)
		{
			case 0:
				switch(dir)
				{
					case 0:
						frontFaceIndex = 4;
						break;
					case 1:
						frontFaceIndex = 3;
						break;
					case 2:
						frontFaceIndex = 5;
						break;
					case 3:
						frontFaceIndex = 2;
						break;
				}
				break;
			case 1:
				switch (dir)
				{
					case 0:
						frontFaceIndex = 5;
						break;
					case 1:
						frontFaceIndex = 3;
						break;
					case 2:
						frontFaceIndex = 4;
						break;
					case 3:
						frontFaceIndex = 2;
						break;
				}
				break;
			case 2:
				switch (dir)
				{
					case 0:
						frontFaceIndex = 4;
						break;
					case 1:
						frontFaceIndex = 0;
						break;
					case 2:
						frontFaceIndex = 5;
						break;
					case 3:
						frontFaceIndex = 1;
						break;
				}
				break;
			case 3:
				switch (dir)
				{
					case 0:
						frontFaceIndex = 4;
						break;
					case 1:
						frontFaceIndex = 1;
						break;
					case 2:
						frontFaceIndex = 5;
						break;
					case 3:
						frontFaceIndex = 0;
						break;
				}
				break;
			case 4:
				switch (dir)
				{
					case 0:
						frontFaceIndex = 1;
						break;
					case 1:
						frontFaceIndex = 3;
						break;
					case 2:
						frontFaceIndex = 0;
						break;
					case 3:
						frontFaceIndex = 2;
						break;
				}
				break;
			case 5:
				switch (dir)
				{
					case 0:
						frontFaceIndex = 0;
						break;
					case 1:
						frontFaceIndex = 3;
						break;
					case 2:
						frontFaceIndex = 1;
						break;
					case 3:
						frontFaceIndex = 2;
						break;
				}
				break;
		}

		return frontFaceIndex;
	}

	private Face GetFrontFace()
	{
		return faces[GetFrontFaceIndex()];
	}

	private Vector3 GetFrontFaceDirection()
	{
		Vector3 t = Vector3.zero;
		switch (GetFrontFaceIndex())
		{
			case 0:
				t = u.transform.position;
				break;
			case 1:
				t = d.transform.position;
				break;
			case 2:
				t = l.transform.position;
				break;
			case 3:
				t = r.transform.position;
				break;
			case 4:
				t = f.transform.position;
				break;
			case 5:
				t = b.transform.position;
				break;
		}
		return t - transform.position;
	}

	private Vector3 SnapVector(Vector3 v)
	{
		
		if (Mathf.Abs(v.x) > Mathf.Abs(v.y) && Mathf.Abs(v.x) > Mathf.Abs(v.z))
		{
			return new Vector3(v.x, 0, 0).normalized;
		}

		if (Mathf.Abs(v.y) > Mathf.Abs(v.x) && Mathf.Abs(v.y) > Mathf.Abs(v.z))
		{
			return new Vector3(0, v.y, 0).normalized;
		}

		if (Mathf.Abs(v.z) > Mathf.Abs(v.x) && Mathf.Abs(v.z) > Mathf.Abs(v.y))
		{
			return new Vector3(0, 0, v.z).normalized;
		}

		return Vector3.zero;
	}

	private Vector3 AlignRotation(Vector3 rot)
	{
		//Debug.Log("pre: " + rot);
		int[] snapAngles = { 0, 90, 180, 270, 360 };
		for (int i = 0; i < snapAngles.Length; i++)
		{
			for (int v = 0; v < 3; v++)
			{
				float c;
				if (v == 0) 
					c = rot.x;
				else if (v == 1) 
					c = rot.y;
				else 
					c = rot.z;

				if (Mathf.Abs(snapAngles[i] - Mathf.Abs(c)) <= rotationAlignThreshold && Mathf.Abs(snapAngles[i] - Mathf.Abs(c)) > aboutZero)
				{
					float snapAngle = snapAngles[i] * Mathf.Sign(c);
					float dist = Mathf.Abs(snapAngles[i] - Mathf.Abs(c));
					c = Mathf.Lerp(c, snapAngle, (dist - aboutZero) / dist);
				}

				if (v == 0)
					rot.x = c;
				else if (v == 1)
					rot.y = c;
				else
					rot.z = c;
			}
		}
		//Debug.Log("post: " + rot);
		return rot;
	}

	private void PaintPips()
	{
		PipPainter[] pps = GetComponentsInChildren<PipPainter>();
		for (int i = 0; i < pps.Length; i++)
		{
			if (randomize)
			{
				faces[i].dir = Random.Range(0, 4);
				faces[i].value = Random.Range(1, 7);
				faces[i].pip = "dot";
			}
			pps[i].SetDir(faces[i].dir);
			pps[i].SetValue(faces[i].value);
			pps[i].SetPip(faces[i].pip);
		}
	}


	[System.Serializable]
	public class Face
	{
		[Range(0, 3)] public int dir; // 0 = up, 1 = right, 2 = down, 3 = left, representing which direction the die will roll
		[Range(1, 6)] public int value; // 1-6, representing the force with which the die will roll.
		public string pip; // the name of the symbol of the face, representing its ability
	}
}
