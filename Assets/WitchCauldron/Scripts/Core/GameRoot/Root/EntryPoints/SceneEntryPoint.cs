using UnityEngine;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.EntryPoints
{
    public abstract class SceneEntryPoint : MonoBehaviour
    {
        public abstract void Run(DiContainer container);
    }
}