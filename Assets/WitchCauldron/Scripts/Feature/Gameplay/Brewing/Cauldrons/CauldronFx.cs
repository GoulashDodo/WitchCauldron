using R3;
using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.Cauldrons
{
    
    [RequireComponent(typeof(Cauldron))]
    public class CauldronFx : MonoBehaviour
    {
        private Cauldron _cauldron;
        private SpriteRenderer _spriteRenderer;
        
        private void Awake()
        {
            _cauldron = GetComponent<Cauldron>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        

        private void ChangeFx(bool hasBrewingSession)
        {
            Debug.Log("Changing fx");
            _spriteRenderer.color = hasBrewingSession ? Color.white : Color.red;
        }
        
    }
}