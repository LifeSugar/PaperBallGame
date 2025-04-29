using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public TrashType trashType;
}

[System.Serializable]
public enum TrashType
{
    RECYCLABE = 1,
    UNRECYCLABLE = 2
}
