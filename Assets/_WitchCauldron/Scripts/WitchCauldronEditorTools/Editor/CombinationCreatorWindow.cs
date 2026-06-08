using Gameplay.Items.Combination.ScriptableObjects;
using Gameplay.Items.SO;
using UnityEditor;
using UnityEngine;

namespace WitchCauldronEditorTools.Editor
{
    public class CombinationCreatorWindow : EditorWindow
    {
        private const string DefaultCombinationFolder = "Assets/_WitchCauldron/Settings/Data/Game/2.Gameplay/Items/Combinations";

        [SerializeField] private CombinationRuleList _ruleList;
        [SerializeField] private ItemSettings _itemA;
        [SerializeField] private ItemSettings _itemB;
        [SerializeField] private ItemSettings _result;
        [SerializeField] private bool _replaceExistingCombination = true;

        [MenuItem("Tools/Witch Cauldron/Combination Creator")]
        public static void Open()
        {
            GetWindow<CombinationCreatorWindow>("Combination Creator");
        }

        private void OnEnable()
        {
            _ruleList ??= FindDefaultRuleList();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Rule List", EditorStyles.boldLabel);
            _ruleList = (CombinationRuleList)EditorGUILayout.ObjectField("Combination Rule List", _ruleList, typeof(CombinationRuleList), false);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Combination", EditorStyles.boldLabel);
            _itemA = (ItemSettings)EditorGUILayout.ObjectField("Item A", _itemA, typeof(ItemSettings), false);
            _itemB = (ItemSettings)EditorGUILayout.ObjectField("Item B", _itemB, typeof(ItemSettings), false);
            _result = (ItemSettings)EditorGUILayout.ObjectField("Result", _result, typeof(ItemSettings), false);
            _replaceExistingCombination = EditorGUILayout.Toggle("Replace Existing", _replaceExistingCombination);

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(!CanCreateCombination()))
            {
                if (GUILayout.Button("Create Combination"))
                    CreateCombination();
            }
        }

        private bool CanCreateCombination()
        {
            return _ruleList != null &&
                   _itemA != null &&
                   _itemB != null &&
                   _result != null;
        }

        private void CreateCombination()
        {
            var serialized = new SerializedObject(_ruleList);
            var rules = serialized.FindProperty("_rules");

            if (rules == null)
            {
                Debug.LogError("Combination rule list does not contain '_rules' property.");
                return;
            }

            var existingIndex = FindExistingRuleIndex(rules, _itemA, _itemB);

            if (existingIndex >= 0)
            {
                if (!_replaceExistingCombination)
                {
                    Debug.LogWarning($"Combination '{_itemA.TypeId}' + '{_itemB.TypeId}' already exists.");
                    return;
                }

                WriteRule(rules.GetArrayElementAtIndex(existingIndex), _itemA, _itemB, _result);
            }
            else
            {
                rules.arraySize++;
                WriteRule(rules.GetArrayElementAtIndex(rules.arraySize - 1), _itemA, _itemB, _result);
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(_ruleList);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = _ruleList;
            EditorGUIUtility.PingObject(_ruleList);
        }

        private static int FindExistingRuleIndex(SerializedProperty rules, ItemSettings itemA, ItemSettings itemB)
        {
            for (var i = 0; i < rules.arraySize; i++)
            {
                var rule = rules.GetArrayElementAtIndex(i);
                var existingA = rule.FindPropertyRelative("_itemA").objectReferenceValue as ItemSettings;
                var existingB = rule.FindPropertyRelative("_itemB").objectReferenceValue as ItemSettings;

                if ((existingA == itemA && existingB == itemB) ||
                    (existingA == itemB && existingB == itemA))
                {
                    return i;
                }
            }

            return -1;
        }

        private static void WriteRule(SerializedProperty rule, ItemSettings itemA, ItemSettings itemB, ItemSettings result)
        {
            rule.FindPropertyRelative("_itemA").objectReferenceValue = itemA;
            rule.FindPropertyRelative("_itemB").objectReferenceValue = itemB;
            rule.FindPropertyRelative("_result").objectReferenceValue = result;
        }

        private static CombinationRuleList FindDefaultRuleList()
        {
            var guids = AssetDatabase.FindAssets("t:CombinationRuleList", new[] { DefaultCombinationFolder });

            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets("t:CombinationRuleList");

            if (guids.Length == 0)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<CombinationRuleList>(path);
        }
    }
}
