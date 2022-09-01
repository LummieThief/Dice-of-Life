using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorManager : MonoBehaviour
{
	public static BehaviorManager instance;
	public List<BehaviorBean> behaviors;
	public Dictionary<string, Behavior> blt;
	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);

		BuildDict();
	}

	public void BuildDict()
	{
		blt = new Dictionary<string, Behavior>();
		foreach(BehaviorBean bean in behaviors)
		{
			blt.Add(bean.pip, bean.behaviorScript);
		}
	}

	public Behavior GetBehavior(string name)
	{
		return blt[name];
	}

	[System.Serializable]
	public class BehaviorBean
	{
		public Behavior behaviorScript;
		public string pip;
	}
}


