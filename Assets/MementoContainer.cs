using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class MementoContainer : MonoBehaviour
{
    [SerializeField] private Memento memento;

    [YarnCommand("give_memento")]
    public void GiveToPlayer() {
        PlayerSingleton.PlayerSing.Play.SetSpell(memento);
    }
}
