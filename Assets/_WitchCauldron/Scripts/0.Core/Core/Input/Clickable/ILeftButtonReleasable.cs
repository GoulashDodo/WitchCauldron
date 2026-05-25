using UnityEngine;

namespace Core.Input.Clickable
{
    public interface ILeftButtonReleasable
    {
        void OnLeftButtonReleased(Vector3 mousePosition);
    }
}