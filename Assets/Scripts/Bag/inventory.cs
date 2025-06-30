using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New inventory", menuName = "inventory/New inventory")]
public class inventory : ScriptableObject
{
    public List<item> itemList = new List<item>();
}
