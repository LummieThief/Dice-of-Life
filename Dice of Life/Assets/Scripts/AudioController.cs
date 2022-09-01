using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public List<AudioSource> sources;
    public static AudioController instance;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
        else
		{
            Destroy(gameObject);
		}

    }

    public AudioSource GetSound(int index)
	{
        return sources[index];
	}
}
