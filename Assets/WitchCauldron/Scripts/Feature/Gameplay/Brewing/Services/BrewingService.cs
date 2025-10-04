using System.Collections.Generic;
using System.Linq;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.Cauldrons;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.Services
{
    public class BrewingService
    {
        private readonly ReceiptService _receiptService;

        private readonly Dictionary<int, BrewingSession> _allBrewingSessions = new();

        public BrewingSession CurrentBrewingSession => _allBrewingSessions.FirstOrDefault().Value;
        
        
        public BrewingService(ReceiptService receiptService)
        {
            _receiptService = receiptService;
        }
        
        
        public BrewingSession CreateBrewingSession()
        {
            var session = new BrewingSession(_receiptService.GetRandomBrewingReceipt());
            
            _allBrewingSessions.Add(session.GetHashCode(),  session);
            return session;
        }
        
        public bool TryDequeueIngredient(BrewingSession session, BrewingIngredient ingredient)
        {
            return session.TryAddIngredient(ingredient);
        }
    }
}