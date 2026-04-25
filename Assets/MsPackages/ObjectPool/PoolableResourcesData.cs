using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PoolableResource
{
    public EPoolableType poolableType;
    public GameObject poolableObject;
}

[CreateAssetMenu(fileName = "PoolableResourcesData", menuName = "Scriptable Object/Resource Data/Poolable Resources Data")]
public class PoolableResourcesData : ScriptableObject
{
    public List<PoolableResource> poolableResourceList = new List<PoolableResource>();
}