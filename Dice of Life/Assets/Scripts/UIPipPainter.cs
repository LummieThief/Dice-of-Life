using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPipPainter : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] Image[] pips = new Image[7];
	[SerializeField] Image[] arrows = new Image[4];
	[SerializeField] Image bg;
	[SerializeField] Color bgSelected, bgDefault;
	[SerializeField] List<SpriteBean> sprites;
	private DieCustomizer customizer;
	private Dictionary<string, Sprite> slt;
	[SerializeField] int index;

	private void Awake()
	{
		BuildDict();
	}

	public void Subscribe(DieCustomizer customizer)
	{
		this.customizer = customizer;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (customizer != null)
		{
			customizer.FaceClicked(index);
		}
	}

	public void BuildDict()
	{
		slt = new Dictionary<string, Sprite>();
		foreach (SpriteBean bean in sprites)
		{
			slt.Add(bean.name, bean.sprite);
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
		switch (val)
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
		for (int i = 0; i < pips.Length; i++)
		{
			pips[i].sprite = slt[name];
		}
	}
	public Vector3 GetArrowPosition(int dir)
	{
		return arrows[dir].transform.position;
	}
	public void SetActive(bool activeState)
	{
		if (activeState)
		{
			bg.color = bgSelected;
		}
		else
		{
			bg.color = bgDefault;
		}
	}

	[System.Serializable]
	public class SpriteBean
	{
		public Sprite sprite;
		public string name;
	}
}
