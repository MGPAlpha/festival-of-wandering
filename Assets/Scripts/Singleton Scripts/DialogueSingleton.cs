using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueSingleton : MonoBehaviour
{
    public static DialogueSingleton Dialogue { get; private set; }
    public DialogueRunner Runner {get; private set;}
    public static GameObject respondentImageGO;
    public static Image RespondentImage {get; private set;}

    [SerializeField]
    public List<Sprite> spriteList;
    public static List<Sprite> dummySpriteList;

    [YarnCommand("changeRespondantImg")]
    public static void ChangeRespondantImage(string img){
        if (img == "null") {
            respondentImageGO.SetActive(false);
        } else {
            respondentImageGO.SetActive(true);
            RespondentImage.sprite = dummySpriteList.Find(sprite => sprite.name == img);
        }
    }

    private Sprite GetSprite( string img){
        System.Type type = GetType();
        FieldInfo info = type.GetField(img);
        return (Sprite)info.GetValue(img);
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        Dialogue = this;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        Runner = GetComponent<DialogueRunner>();
        dummySpriteList = spriteList;
        respondentImageGO = GameObject.Find("RespondantImage");
        RespondentImage = respondentImageGO.GetComponent<Image>();
    }
}