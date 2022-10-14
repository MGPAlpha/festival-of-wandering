using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistBarrier : MonoBehaviour
{
    private SpriteRenderer _sp;
    private BoxCollider2D _col;

    [SerializeField] private bool mistActive = false;
    private float mistLevel = 0;
    [SerializeField] private float mistFadetime = 1;
    
    bool started = false;
    bool mistActiveDeferred = false;

    // Start is called before the first frame update
    void Start()
    {
        started = true;
        _sp = GetComponent<SpriteRenderer>();
        _col = GetComponent<BoxCollider2D>();
        _col.enabled = mistActive;
        _sp.material.SetFloat("_Opacity", mistActive ? 1 : 0);
        mistLevel = mistActive ? 1 : 0;
        SetMistActive(mistActiveDeferred);
    }

    // Update is called once per frame
    void Update()
    {
        if (mistActive) mistLevel += Time.deltaTime;
        else mistLevel -= Time.deltaTime;
        mistLevel = Mathf.Clamp(mistLevel, 0, mistFadetime);
        _sp.material.SetFloat("_Opacity", mistLevel / mistFadetime);
    }

    public void SetMistActive(bool active) {
        if (started) { 
            _col.enabled = active;
            mistActive = active;
        } else {mistActiveDeferred = active;}
    }
}
