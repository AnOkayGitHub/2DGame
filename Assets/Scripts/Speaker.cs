#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Speaker", menuName = "Dialogue/New Speaker")]
public class Speaker : ScriptableObject {
    
    [SerializeField] private string speakerName;
    [SerializeField] private Sprite[] speakerSprites;

    public string GetName() {
        return speakerName;
    }

    public Sprite[] GetSprite() {
        return speakerSprites;
    }
}

