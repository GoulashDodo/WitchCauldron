using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Clickable
{
    public interface ILeftButtonReleasable
    {
        void OnLeftButtonReleased(Vector3 mousePosition);
    }
}