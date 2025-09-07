using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Root.EntryPoints.GameplayEntryPoint.Registrations;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.EntryPoints.GameplayEntryPoint
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        
        [SerializeField] private PotionReceiptList _receipts;
        
        public void Run(DiContainer sceneContainer)
        {
            GameplayRegistrations.Register(sceneContainer, _receipts);
            
        }
        
    }
}
