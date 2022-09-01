using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipPainter : MonoBehaviour
{
	[SerializeField] MeshRenderer[] pips = new MeshRenderer[7];
	[SerializeField] MeshRenderer[] arrows = new MeshRenderer[4];
	[SerializeField] List<MaterialBean> mats;
	private Dictionary<string, Material> mlt;

	private void Awake()
	{
		BuildDict();
	}
	public void BuildDict()
	{
		mlt = new Dictionary<string, Material>();
		foreach (MaterialBean bean in mats)
		{
			mlt.Add(bean.name, bean.mat);
		}
	}
	public void SetDir(int dir)
	{
		for (int i = 0; i < arrows.Length; i++)
		{
			arrows[i].enabled = i == dir;
		}
	}
	public void SetValue(int val)
	{
		HashSet<int> pps = new HashSet<int>();
		switch(val)
		{
			case 1:
				pps.Add(3);
				break;
			case 2:
				pps.Add(1);
				pps.Add(5);
				break;
			case 3:
				pps.Add(1);
				pps.Add(3);
				pps.Add(5);
				break;
			case 4:
				pps.Add(0);
				pps.Add(1);
				pps.Add(5);
				pps.Add(6);
				break;
			case 5:
				pps.Add(0);
				pps.Add(1);
				pps.Add(3);
				pps.Add(5);
				pps.Add(6);
				break;
			case 6:
				pps.Add(0);
				pps.Add(1);
				pps.Add(2);
				pps.Add(4);
				pps.Add(5);
				pps.Add(6);
				break;
		}
		for (int i = 0; i < pips.Length; i++)
		{
			pips[i].enabled = pps.Contains(i);
		}
	}
	public void SetPip(string name)
	{
		for(int i = 0; i < pips.Length; i++)
		{
			pips[i].sharedMaterial = mlt[name];
		}
	}
	public Vector3 GetArrowPosition(int dir)
	{
		return arrows[dir].transform.position;
	}

	[System.Serializable]
	public class MaterialBean
	{
		public Material mat;
		public string name;
	}
}
