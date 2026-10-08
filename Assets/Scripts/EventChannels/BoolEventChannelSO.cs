using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolEventChannelSO", menuName = "Events/BoolEventChannel")]
public class BoolEventChannelSO : ScriptableObject
{

    public event Action<bool> OnEventRaised;

    public void Raise(bool value)
    {

        OnEventRaised?.Invoke(value);
    }

}


