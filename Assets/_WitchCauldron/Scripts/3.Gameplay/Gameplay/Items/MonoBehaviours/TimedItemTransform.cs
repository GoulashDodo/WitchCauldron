using System.Collections;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DraggableItem))]
    public sealed class TimedItemTransform : MonoBehaviour
    {
        [SerializeField] private string _replacementItemTypeId;
        [SerializeField] private float _delay = 5f;
        [SerializeField] private bool _preserveDragging;

        private DraggableItem _item;
        private Coroutine _transformCoroutine;

        private void Awake()
        {
            _item = GetComponent<DraggableItem>();
        }

        private void OnEnable()
        {
            _transformCoroutine = StartCoroutine(TransformAfterDelay());
        }

        private void OnDisable()
        {
            if (_transformCoroutine == null)
                return;

            StopCoroutine(_transformCoroutine);
            _transformCoroutine = null;
        }

        private IEnumerator TransformAfterDelay()
        {
            yield return new WaitForSeconds(Mathf.Max(0f, _delay));

            _transformCoroutine = null;

            if (_item == null)
                yield break;

            _item.TryTransformTo(_replacementItemTypeId, _preserveDragging);
        }
    }
}
