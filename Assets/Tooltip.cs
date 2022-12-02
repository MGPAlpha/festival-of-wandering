using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Tooltip : MonoBehaviour
{

    [SerializeField] private bool startVisible = false;
    [SerializeField] private float fadeSpeed = 2f;

    private float alpha;
    private bool visible;

    private CanvasGroup _cg;

    // Start is called before the first frame update
    void Start()
    {
        _cg = GetComponent<CanvasGroup>();

        if (startVisible) {
            visible = true;
            alpha = 1;
        }
        else {
            alpha = 0;
            visible = false;
        }

        _cg.alpha = alpha;
    }

    // Update is called once per frame
    void Update()
    {
        if (visible) {
            alpha += fadeSpeed * Time.deltaTime;
        } else {
            alpha -= fadeSpeed * Time.deltaTime;
        }
        alpha = Mathf.Clamp(alpha, 0, 1);

        _cg.alpha = alpha;
    }

    [YarnCommand("show_tooltip")]
    public void SetVisible(bool visible) {
        this.visible = visible;
    }
}
