using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DieCustomizer : MonoBehaviour
{
    public float lerpSpeed;
    private float remainingRotation;
    private Vector3 axis;

    private int currentFace = 0;
	private int[] dirs = new int[6];
	private int[] vals = { 1, 1, 1, 1, 1, 1 };

	//public DieGimble gimble;
	private PipPainter[] pps;
	public UIPipPainter[] uipps;

	private Die die;

	private int sceneIndex;

	private void Awake()
	{
		sceneIndex = SceneManager.GetActiveScene().buildIndex;
		die = GetComponent<Die>();
		pps = GetComponentsInChildren<PipPainter>();
		uipps = MainCanvas.instance.GetComponentsInChildren<UIPipPainter>();
		for(int i = 0; i < pps.Length; i++)
		{
			pps[i].SetValue(1);
			pps[i].SetDir(0);
		}
		for (int i = 0; i < uipps.Length; i++)
		{
			uipps[i].SetValue(1);
			uipps[i].SetDir(0);
			uipps[i].Subscribe(this);
		}
		if (uipps.Length > 0)
		{
			uipps[0].SetActive(true);
		}
		
	}
	void Update()
    {
		if (MainCanvas.instance.paused)
			return;
		if (Star.numStars != 0)
		{
			HandleDynamicInput();
			if (MainCanvas.instance.dieagramState && !die.running) 
			{
				HandleStaticInput();
			}
		}
    }

	private void HandleDynamicInput()
	{
		if (!die.running && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
		{
			die.TryRoll();
		}

		if (die.running && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)))
		{
			die.Return();
		}
	}

	public void ResetCurrentUIFace()
	{
		if (uipps.Length == 0)
			return;
		uipps[currentFace].SetActive(false);
		currentFace = 0;
		uipps[currentFace].SetActive(true);
	}

	private void HandleStaticInput()
	{
		if (sceneIndex == 1)
		{
			return;
		}

		if (sceneIndex > 2)
		{
			if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && currentFace != 4 && currentFace != 2 && currentFace != 3)
			{
				uipps[currentFace].SetActive(false);
				//BeginRotation(Vector3.left);
				currentFace = GetNextFace(0);
				uipps[currentFace].SetActive(true);
				AudioController.instance.GetSound(4).Play();
			}
			if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && currentFace != 1 && currentFace != 2 && currentFace != 3)
			{
				uipps[currentFace].SetActive(false);
				//BeginRotation(Vector3.right);
				currentFace = GetNextFace(2);
				uipps[currentFace].SetActive(true);
				AudioController.instance.GetSound(4).Play();
			}
			if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && currentFace != 4 && currentFace != 2 && currentFace != 5 && currentFace != 1)
			{
				uipps[currentFace].SetActive(false);
				//BeginRotation(Vector3.back);
				currentFace = GetNextFace(3);
				uipps[currentFace].SetActive(true);
				AudioController.instance.GetSound(4).Play();
			}
			if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && currentFace != 4 && currentFace != 3 && currentFace != 5 && currentFace != 1)
			{
				uipps[currentFace].SetActive(false);
				//BeginRotation(Vector3.forward);
				currentFace = GetNextFace(1);
				uipps[currentFace].SetActive(true);
				AudioController.instance.GetSound(4).Play();
			}

			if (sceneIndex > 3)
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					UpdateDir(RotateDir(dirs[currentFace], 1));
					AudioController.instance.GetSound(6).Play();
				}
			}
		}


		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
		{
			UpdateValue(1);
			AudioController.instance.GetSound(5).Play();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
		{
			UpdateValue(2);
			AudioController.instance.GetSound(5).Play();
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
		{
			UpdateValue(3);
			AudioController.instance.GetSound(5).Play();
		}
		if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
		{
			UpdateValue(4);
			AudioController.instance.GetSound(5).Play();
		}
		if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
		{
			UpdateValue(5);
			AudioController.instance.GetSound(5).Play();
		}
		if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
		{
			UpdateValue(6);
			AudioController.instance.GetSound(5).Play();
		}

		if (Input.mouseScrollDelta.y < 0)
		{
			IncrementValue(true);
		}
		else if (Input.mouseScrollDelta.y > 0)
		{
			IncrementValue(false);
		}
	}

	private void UpdateValue(int val)
	{
		pps[currentFace].SetValue(val);
		uipps[currentFace].SetValue(val);
		die.faces[currentFace].value = val;

		vals[currentFace] = val;
	}
	private void IncrementValue(bool decrement)
	{
		int v = vals[currentFace];
		if (decrement)
		{
			v--;
			if (v == 0)
			{
				v = 1;
			}
			else
			{
				AudioController.instance.GetSound(6).Play();
			}
		}
		else
		{
			v++;
			if (v == 7)
			{
				v = 6;
			}
			else
			{
				AudioController.instance.GetSound(5).Play();
			}
		}
		pps[currentFace].SetValue(v);
		uipps[currentFace].SetValue(v);
		die.faces[currentFace].value = v;

		vals[currentFace] = v;
	}
	private void UpdateDir(int dir)
	{
		dirs[currentFace] = dir;
		pps[currentFace].SetDir(dirs[currentFace]);
		uipps[currentFace].SetDir(dirs[currentFace]);
		die.faces[currentFace].dir = dir;
	}

	private int RotateDir(int d1, int d2)
	{
		d1 += d2;
		if (d1 > 3)
		{
			d1 -= 4;
		}
		return d1;
	}

	private int GetNextFace(int dir)
	{
		switch (currentFace)
		{
			case 0:
				if (dir == 0) { return 4; }
				else if (dir == 1) { return 3; }
				else if (dir == 2) { return 5; }
				else { return 2; }
			case 1:
				return 5;
			case 2:
				return 0;
			case 3:
				return 0;
			case 4:
				return 0;
			case 5:
				if (dir == 0) { return 0; }
				else { return 1; }
		}
		return -1;
	}

	public void FaceClicked(int index)
	{

		if (!die.running && sceneIndex > 2)
		{
			if (currentFace == index)
			{
				if (sceneIndex > 3)
				{
					UpdateDir(RotateDir(dirs[currentFace], 1));
					AudioController.instance.GetSound(6).Play();
				}
			}
			else
			{
				uipps[currentFace].SetActive(false);
				currentFace = index;
				uipps[currentFace].SetActive(true);
				AudioController.instance.GetSound(4).Play();
			}
		}
	}
}
