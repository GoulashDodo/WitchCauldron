using Core.SceneManagement;
using Gameplay._root;
using Gameplay.Items.Services;
using UnityEngine;
using Zenject;

namespace Gameplay.UI.SpawnButtons
{
    public class UISpawnButtonParent : MonoBehaviour
    {
        [SerializeField] private UISpawnButton _buttonPf;


        //TODO: change, test purpose only
        [Inject]
        public void Construct(SceneParametersPayload payload, ItemService itemService)
        {
            Initialize(payload.GameplayEntryParameters, itemService);
        }
        
        
        public void Initialize(GameplayEntryParameters entryParameters, ItemService itemService)
        {
            foreach (var itemId in entryParameters.SelectedItemsIds)
            {
                var button = Instantiate(_buttonPf, gameObject.transform, false);
                button.Initialize(itemId, itemService);
            }
        }
        
        
    }
}