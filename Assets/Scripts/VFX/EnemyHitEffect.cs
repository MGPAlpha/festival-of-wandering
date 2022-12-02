using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitEffect : MonoBehaviour
{
    [SerializeField, Tooltip("Number of flickers")] private int numFlickers = 1;
    [SerializeField, Tooltip("How much time to do one flicker")] private float flickerDuration = 0.25f;
    [SerializeField] private Material spriteFillMat;
    [SerializeField] private GameObject hitParticles;

    private Material _savedMaterial;
    private SpriteRenderer _sr;
    private Coroutine _flickerCoroutine = null;
    private GameObject _particlesInstance;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _savedMaterial = _sr.material;
    }

    private void OnDestroy()
    {
        if (_flickerCoroutine != null)
        {
            StopCoroutine(_flickerCoroutine);
        }
    }

    public void PlayEffect()
    {
        _particlesInstance = Instantiate(hitParticles, this.transform);
        _particlesInstance.GetComponentInChildren<ParticleSystem>().Play();
        _flickerCoroutine = StartCoroutine(FlickerCoroutine());
    }

    private IEnumerator FlickerCoroutine()
    {
        int count = 0;
        while (count < numFlickers)
        {
            // Debug.Log("Flicker On");
            _sr.material = spriteFillMat;
            yield return new WaitForSeconds(flickerDuration);
            _sr.material = _savedMaterial;
            // Debug.Log("Flicker Off");
            yield return new WaitForSeconds(flickerDuration);
            count++;
        }
        yield return null;
    }
}
