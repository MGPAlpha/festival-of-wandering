using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NPCYarnScripts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [YarnCommand("walk_to")]
    public IEnumerator WalkTo(GameObject gameObject, float time) {
        float finalTime = Time.time + time;
        float totalTime = 0;
        Vector2 startPosition = transform.position;
        Vector2 finalPosition = gameObject.transform.position;
        while (Time.time < finalTime) {
            transform.position = Vector2.Lerp(startPosition, finalPosition, totalTime / time);
            totalTime += Time.deltaTime;
            yield return null;
        }
    }

    [YarnCommand("teleport_to")]
    public void TeleportTo(GameObject gameObject) {
        transform.position = gameObject.transform.position;
    }
}
