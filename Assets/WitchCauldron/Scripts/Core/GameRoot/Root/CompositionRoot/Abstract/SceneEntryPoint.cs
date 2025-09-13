using UnityEngine;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Abstract
{
    public abstract class SceneEntryPoint : MonoBehaviour
    {
        public abstract void Run(DiContainer container);
    }
}