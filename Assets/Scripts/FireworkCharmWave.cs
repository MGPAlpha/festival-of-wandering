using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkCharmWave : MonoBehaviour
{

    [SerializeField] private AnimationCurve waveSpread;
    [SerializeField] private float spreadTime = 3f;
    [SerializeField] private float spreadRadius = 10;
    private float time = 0f;

    private static Vector3 baseScale = new Vector3(1,1,1);


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        transform.localScale = waveSpread.Evaluate(time/spreadTime) * spreadRadius * baseScale;
        if (time > spreadTime) Destroy(this.gameObject);
    }
}
