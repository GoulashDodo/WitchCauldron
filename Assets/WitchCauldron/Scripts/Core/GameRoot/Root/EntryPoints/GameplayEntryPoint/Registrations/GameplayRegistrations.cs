using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Services;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.EntryPoints.GameplayEntryPoint.Registrations
{
    public static class GameplayRegistrations
    {
        public static void Register(DiContainer container, PotionReceiptList receiptList)
        {
            container.Bind<PotionReceiptList>().FromInstance(receiptList).AsSingle();
            
            container.Bind<ReceiptService>().AsSingle();

            container.Bind<BrewingService>().AsSingle();

        }
        
        
    }
}