using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Clickable
{
    public interface ILeftButtonReleasable
    {
        void OnLeftButtonReleased(Vector3 mousePosition);
    }
}