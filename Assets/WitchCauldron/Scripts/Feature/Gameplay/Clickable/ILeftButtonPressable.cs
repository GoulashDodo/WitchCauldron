using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Clickable
{
    public interface ILeftButtonPressable
    {
        void OnLeftButtonPressed(Vector3 mousePosition);
        
    }
}