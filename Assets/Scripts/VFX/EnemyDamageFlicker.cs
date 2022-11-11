using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageFlicker : MonoBehaviour
{
    SpriteRenderer _sr;

    [Tooltip("Number of flickers")] [SerializeField] private int numFlickers = 2;
    [Tooltip("How much time to do one flicker")] [SerializeField] private float flickerDuration = 0.25f;
    [SerializeField] private Color flickerColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

    private Coroutine flickerCoroutine = null;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
        }
    }

    public void Flicker()
    {
        flickerCoroutine = StartCoroutine(FlickerCoroutine());
    }

    private IEnumerator FlickerCoroutine()
    {
        int count = 0;
        while (count < numFlickers)
        {
            Debug.Log("Flicker On");
            _sr.color = flickerColor;
            yield return new WaitForSeconds(flickerDuration);
            _sr.color = Color.white;
            Debug.Log("Flicker Off");
            yield return new WaitForSeconds(flickerDuration);
            count++;
        }
        yield return null;
    }
}
