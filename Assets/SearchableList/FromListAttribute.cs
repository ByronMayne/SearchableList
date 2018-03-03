using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class FromListAttribute : PropertyAttribute
{
    private bool _fuzzySearch;
    
    public bool fuzzySearch
    {
        get { return _fuzzySearch;  }
        set { _fuzzySearch = value; }
    }
}
