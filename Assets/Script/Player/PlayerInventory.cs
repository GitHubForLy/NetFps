using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<int> Inventory = new List<int>();
    void Start()
    {
        
    }

    public void AddKeyId(int id)
    {
        Inventory.Add(id);
    }


    /// <summary>
    /// 是否存在物品id
    /// </summary>
    public bool HasKeyId(int id)
    {
        return Inventory.Contains(id);
    }
}
