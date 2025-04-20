using UnityEngine;
using System.Collections.Generic;

public class VelocityModifierSystem : MonoBehaviour
{
    private List<Vector3> _activeModifiers = new List<Vector3>();

    public Vector3 GetModifiers()
    {
        Vector3 modifiedTotal = Vector3.zero;

        // Apply all active modifiers
        for (int i = _activeModifiers.Count - 1; i >= 0; i--)
        {
            modifiedTotal += _activeModifiers[i];
            _activeModifiers.RemoveAt(i);
        }

        return modifiedTotal;
    }

    public void AddModifier(Vector3 modifier)
    {
        _activeModifiers.Add(modifier);
    }
}