using UnityEngine;

namespace Core.GameRoot.Input.Clickable
{
    public interface ILeftButtonReleasable
    {
        void OnLeftButtonReleased(Vector3 mousePosition);
    }
}