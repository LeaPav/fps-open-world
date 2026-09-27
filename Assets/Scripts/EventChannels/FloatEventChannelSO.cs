using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatEventChannelSO", menuName = "Events/FloatEventChannel")]
public class FloatEventChannelSO : ScriptableObject
{
    // "Action<float>" : délégué générique, qui force que toute methode abonnée ait la signature float, ne retournant rien.
    public event Action<float> OnEventRaised;

    public void Raise(float value)
    {
        // "?." : operateur de null-conditionnel. OnEventRaised est un event, donc tant qu'il n'y a pas d'abonne avec +=, la valeur est null
        // Si on enleve le ?. et qu'aucun script ne s'est encore abonné, le jeu crash (NullReferenceExeception)
        OnEventRaised?.Invoke(value);
    }
}
