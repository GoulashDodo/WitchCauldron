using Gameplay.Items.MonoBehaviours;
using Gameplay.Items.SO;
using Gameplay.Items.Usable.Commands;
using Hut.SO;
using UnityEditor;
using UnityEngine;

namespace WitchCauldronEditorTools.Editor
{
    public class ItemCreatorWindow : EditorWindow
    {
        private const string DefaultItemSettingsFolder = "Assets/_WitchCauldron/Settings/Data/Game/2.Gameplay/Items";
        private const string DefaultItemPrefabFolder = "Assets/_WitchCauldron/Prefabs/Gameplay/Items/DraggableItems";
        private const string DefaultSelectableItemsFolder = "Assets/_WitchCauldron/Settings/Data/Game/1.Hut";

        [SerializeField] private string _typeId = "Item_New";
        [SerializeField] private string _titleLid = "New Item";
        [SerializeField] private float _spawnCooldown = 5f;
        [SerializeField] private Sprite _icon;

        [SerializeField] private bool _createPrefabFromTemplate = true;
        [SerializeField] private DraggableItem _templateItemPrefab;
        [SerializeField] private DraggableItem _itemPrefab;

        [SerializeField] private UseCommandParameters[] _onUseCommands = new UseCommandParameters[0];

        [SerializeField] private DefaultAsset _settingsOutputFolder;
        [SerializeField] private DefaultAsset _prefabOutputFolder;
        [SerializeField] private AllItemSettings _allItemSettings;
        [SerializeField] private bool _appendToAllItemSettings = true;
        [SerializeField] private bool _replaceExistingTypeInAllItemSettings = true;

        [SerializeField] private AllSelectableItems _allSelectableItems;
        [SerializeField] private bool _appendToSelectableItems;
        [SerializeField] private bool _replaceExistingTypeInSelectableItems = true;

        [MenuItem("Tools/Witch Cauldron/Item Creator")]
        public static void Open()
        {
            GetWindow<ItemCreatorWindow>("Item Creator");
        }

        private void OnEnable()
        {
            _templateItemPrefab ??= FindDefaultItemPrefab();
            _allItemSettings ??= FindDefaultAllItemSettings();
            _allSelectableItems ??= FindDefaultAllSelectableItems();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            _typeId = EditorGUILayout.TextField("Type Id", _typeId);
            _titleLid = EditorGUILayout.TextField("Title Lid", _titleLid);
            _spawnCooldown = EditorGUILayout.FloatField("Spawn Cooldown", _spawnCooldown);
            _icon = (Sprite)EditorGUILayout.ObjectField("Icon", _icon, typeof(Sprite), false);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel);
            _createPrefabFromTemplate = EditorGUILayout.Toggle("Create Prefab From Template", _createPrefabFromTemplate);

            using (new EditorGUI.DisabledScope(!_createPrefabFromTemplate))
                _templateItemPrefab = (DraggableItem)EditorGUILayout.ObjectField("Template Item Prefab", _templateItemPrefab, typeof(DraggableItem), false);

            using (new EditorGUI.DisabledScope(_createPrefabFromTemplate))
                _itemPrefab = (DraggableItem)EditorGUILayout.ObjectField("Item Prefab", _itemPrefab, typeof(DraggableItem), false);

            EditorGUILayout.Space();
            DrawUseCommands();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            _settingsOutputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Settings Folder", _settingsOutputFolder, typeof(DefaultAsset), false);
            _prefabOutputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Prefab Folder", _prefabOutputFolder, typeof(DefaultAsset), false);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Registries", EditorStyles.boldLabel);
            _allItemSettings = (AllItemSettings)EditorGUILayout.ObjectField("All Item Settings", _allItemSettings, typeof(AllItemSettings), false);
            _appendToAllItemSettings = EditorGUILayout.Toggle("Append To Gameplay List", _appendToAllItemSettings);

            using (new EditorGUI.DisabledScope(!_appendToAllItemSettings))
                _replaceExistingTypeInAllItemSettings = EditorGUILayout.Toggle("Replace Gameplay Type", _replaceExistingTypeInAllItemSettings);

            _allSelectableItems = (AllSelectableItems)EditorGUILayout.ObjectField("All Selectable Items", _allSelectableItems, typeof(AllSelectableItems), false);
            _appendToSelectableItems = EditorGUILayout.Toggle("Append To Hut Selection", _appendToSelectableItems);

            using (new EditorGUI.DisabledScope(!_appendToSelectableItems))
                _replaceExistingTypeInSelectableItems = EditorGUILayout.Toggle("Replace Selectable Type", _replaceExistingTypeInSelectableItems);

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(!CanCreateItem()))
            {
                if (GUILayout.Button("Create Item"))
                    CreateItem();
            }
        }

        private void DrawUseCommands()
        {
            EditorGUILayout.LabelField("Use Commands", EditorStyles.boldLabel);

            var size = Mathf.Max(0, EditorGUILayout.IntField("Size", _onUseCommands?.Length ?? 0));
            if (_onUseCommands == null || size != _onUseCommands.Length)
                System.Array.Resize(ref _onUseCommands, size);

            for (var i = 0; i < _onUseCommands.Length; i++)
            {
                _onUseCommands[i] = (UseCommandParameters)EditorGUILayout.ObjectField(
                    $"Command {i}",
                    _onUseCommands[i],
                    typeof(UseCommandParameters),
                    false);
            }
        }

        private bool CanCreateItem()
        {
            return !string.IsNullOrWhiteSpace(_typeId) &&
                   (!_createPrefabFromTemplate || _templateItemPrefab != null) &&
                   (_createPrefabFromTemplate || _itemPrefab != null) &&
                   _spawnCooldown >= 0f;
        }

        private void CreateItem()
        {
            var settingsFolder = GetFolderPath(_settingsOutputFolder, DefaultItemSettingsFolder);
            var prefabFolder = GetFolderPath(_prefabOutputFolder, DefaultItemPrefabFolder);
            var safeName = ObjectNames.NicifyVariableName(_typeId).Replace(" ", string.Empty);

            EnsureFolderExists(settingsFolder);

            var prefab = _createPrefabFromTemplate
                ? CreateItemPrefab(prefabFolder, safeName)
                : _itemPrefab;

            var itemSettings = CreateItemSettings(settingsFolder, safeName, prefab);

            if (_appendToAllItemSettings && _allItemSettings != null)
                AppendItemSettings(_allItemSettings, itemSettings, _replaceExistingTypeInAllItemSettings);

            if (_appendToSelectableItems && _allSelectableItems != null)
                AppendSelectableItemSettings(itemSettings);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = itemSettings;
            EditorGUIUtility.PingObject(itemSettings);
        }

        private DraggableItem CreateItemPrefab(string prefabFolder, string safeName)
        {
            EnsureFolderExists(prefabFolder);

            var templatePath = AssetDatabase.GetAssetPath(_templateItemPrefab);
            var prefabPath = AssetDatabase.GenerateUniqueAssetPath($"{prefabFolder}/{safeName}.prefab");

            if (!AssetDatabase.CopyAsset(templatePath, prefabPath))
                throw new System.InvalidOperationException($"Failed to copy item prefab from {templatePath} to {prefabPath}.");

            var prefab = AssetDatabase.LoadAssetAtPath<DraggableItem>(prefabPath);
            prefab.gameObject.name = safeName;

            if (_icon != null)
            {
                var spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>(true);
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = _icon;
                    EditorUtility.SetDirty(spriteRenderer);
                }
            }

            EditorUtility.SetDirty(prefab.gameObject);
            return prefab;
        }

        private ItemSettings CreateItemSettings(string settingsFolder, string safeName, DraggableItem prefab)
        {
            var itemSettings = CreateInstance<ItemSettings>();
            var path = AssetDatabase.GenerateUniqueAssetPath($"{settingsFolder}/{safeName}.asset");

            AssetDatabase.CreateAsset(itemSettings, path);

            var serialized = new SerializedObject(itemSettings);
            serialized.FindProperty("<TypeId>k__BackingField").stringValue = _typeId.Trim();
            serialized.FindProperty("<ItemPf>k__BackingField").objectReferenceValue = prefab;
            serialized.FindProperty("<TitleLid>k__BackingField").stringValue = string.IsNullOrWhiteSpace(_titleLid) ? _typeId.Trim() : _titleLid.Trim();
            serialized.FindProperty("<SpawnCooldown>k__BackingField").floatValue = Mathf.Max(0f, _spawnCooldown);
            serialized.FindProperty("<Icon>k__BackingField").objectReferenceValue = _icon;

            var onUseCommands = serialized.FindProperty("<OnUseCommands>k__BackingField");
            onUseCommands.arraySize = _onUseCommands?.Length ?? 0;

            for (var i = 0; i < onUseCommands.arraySize; i++)
                onUseCommands.GetArrayElementAtIndex(i).objectReferenceValue = _onUseCommands[i];

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(itemSettings);

            return itemSettings;
        }

        private static void AppendItemSettings(AllItemSettings allItemSettings, ItemSettings itemSettings, bool replaceExistingType)
        {
            var serialized = new SerializedObject(allItemSettings);
            var settings = serialized.FindProperty("<ItemSettings>k__BackingField");

            if (AppendItemSettings(settings, itemSettings, replaceExistingType))
            {
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(allItemSettings);
            }
        }

        private void AppendSelectableItemSettings(ItemSettings itemSettings)
        {
            var serialized = new SerializedObject(_allSelectableItems);
            var settings = serialized.FindProperty("<ItemSettings>k__BackingField");

            if (AppendItemSettings(settings, itemSettings, _replaceExistingTypeInSelectableItems))
            {
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(_allSelectableItems);
            }
        }

        private static bool AppendItemSettings(SerializedProperty settings, ItemSettings itemSettings, bool replaceExistingType)
        {
            for (var i = 0; i < settings.arraySize; i++)
            {
                var existing = settings.GetArrayElementAtIndex(i).objectReferenceValue as ItemSettings;

                if (existing == itemSettings)
                    return false;

                if (existing != null && existing.TypeId == itemSettings.TypeId)
                {
                    if (!replaceExistingType)
                    {
                        Debug.LogWarning($"Item type '{itemSettings.TypeId}' already exists in target list.");
                        return false;
                    }

                    settings.GetArrayElementAtIndex(i).objectReferenceValue = itemSettings;
                    return true;
                }
            }

            settings.arraySize++;
            settings.GetArrayElementAtIndex(settings.arraySize - 1).objectReferenceValue = itemSettings;
            return true;
        }

        private static DraggableItem FindDefaultItemPrefab()
        {
            var guids = AssetDatabase.FindAssets("Item_Egg t:Prefab", new[] { DefaultItemPrefabFolder });

            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets("t:Prefab", new[] { DefaultItemPrefabFolder });

            if (guids.Length == 0)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<DraggableItem>(path);
        }

        private static AllItemSettings FindDefaultAllItemSettings()
        {
            var guids = AssetDatabase.FindAssets("t:AllItemSettings", new[] { DefaultItemSettingsFolder });

            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets("t:AllItemSettings");

            if (guids.Length == 0)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AllItemSettings>(path);
        }

        private static AllSelectableItems FindDefaultAllSelectableItems()
        {
            var guids = AssetDatabase.FindAssets("t:AllSelectableItems", new[] { DefaultSelectableItemsFolder });

            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets("t:AllSelectableItems");

            if (guids.Length == 0)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AllSelectableItems>(path);
        }

        private static string GetFolderPath(DefaultAsset folderAsset, string fallback)
        {
            if (folderAsset == null)
                return fallback;

            var path = AssetDatabase.GetAssetPath(folderAsset);
            return AssetDatabase.IsValidFolder(path) ? path : fallback;
        }

        private static void EnsureFolderExists(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
                return;

            var parts = folder.Split('/');
            var current = parts[0];

            for (var i = 1; i < parts.Length; i++)
            {
                var next = $"{current}/{parts[i]}";

                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);

                current = next;
            }
        }
    }
}
