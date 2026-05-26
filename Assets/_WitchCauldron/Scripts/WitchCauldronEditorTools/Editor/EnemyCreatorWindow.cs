using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.BattleEntities.Enemies.SO;
using UnityEditor;
using UnityEngine;

namespace WitchCauldronEditorTools.Editor
{
    public class EnemyCreatorWindow : EditorWindow
    {
        private const string DefaultEnemySettingsFolder = "Assets/_WitchCauldron/Settings/Data/Game/Gameplay/Battle/Enemies";
        private const string DefaultEnemyPrefabFolder = "Assets/_WitchCauldron/Prefabs/Gameplay/Enemies";

        [SerializeField] private string _typeId = "Enemy_New";
        [SerializeField] private bool _createPrefabFromTemplate = true;
        [SerializeField] private Enemy _templateEnemyPrefab;
        [SerializeField] private Enemy _enemyPrefab;

        [SerializeField] private int _pointPrice = 1;
        [SerializeField] private float _maxHealth = 10f;
        [SerializeField] private float _maxSpeed = 0.5f;
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _attackDistance = 1f;
        [SerializeField] private float _attackSpeed = 1f;

        [SerializeField] private bool _createLootDefinition;
        [SerializeField] private string _dropItemTypeId = "Item_Egg";
        [SerializeField, Range(0f, 1f)] private float _chanceToDropItem = 1f;

        [SerializeField] private DefaultAsset _settingsOutputFolder;
        [SerializeField] private DefaultAsset _prefabOutputFolder;
        [SerializeField] private AllEnemySettings _allEnemySettings;
        [SerializeField] private bool _appendToAllEnemySettings = true;
        [SerializeField] private bool _replaceExistingTypeInAllEnemySettings = true;

        [MenuItem("Tools/Witch Cauldron/Enemy Creator")]
        public static void Open()
        {
            GetWindow<EnemyCreatorWindow>("Enemy Creator");
        }

        private void OnEnable()
        {
            _templateEnemyPrefab ??= FindDefaultEnemyPrefab();
            _allEnemySettings ??= FindDefaultAllEnemySettings();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Identity & Prefab", EditorStyles.boldLabel);
            _typeId = EditorGUILayout.TextField("Type Id", _typeId);
            _createPrefabFromTemplate = EditorGUILayout.Toggle("Create Prefab From Template", _createPrefabFromTemplate);

            using (new EditorGUI.DisabledScope(!_createPrefabFromTemplate))
                _templateEnemyPrefab = (Enemy)EditorGUILayout.ObjectField("Template Enemy Prefab", _templateEnemyPrefab, typeof(Enemy), false);

            using (new EditorGUI.DisabledScope(_createPrefabFromTemplate))
                _enemyPrefab = (Enemy)EditorGUILayout.ObjectField("Enemy Prefab", _enemyPrefab, typeof(Enemy), false);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
            _pointPrice = EditorGUILayout.IntField("Point Price", _pointPrice);
            _maxHealth = EditorGUILayout.FloatField("Max Health", _maxHealth);
            _maxSpeed = EditorGUILayout.FloatField("Max Speed", _maxSpeed);
            _damage = EditorGUILayout.FloatField("Damage", _damage);
            _attackDistance = EditorGUILayout.FloatField("Attack Distance", _attackDistance);
            _attackSpeed = EditorGUILayout.FloatField("Attack Speed", _attackSpeed);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Loot", EditorStyles.boldLabel);
            _createLootDefinition = EditorGUILayout.Toggle("Create Loot Definition", _createLootDefinition);

            using (new EditorGUI.DisabledScope(!_createLootDefinition))
            {
                _dropItemTypeId = EditorGUILayout.TextField("Drop Item Type Id", _dropItemTypeId);
                _chanceToDropItem = EditorGUILayout.Slider("Chance To Drop Item", _chanceToDropItem, 0f, 1f);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            _settingsOutputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Settings Folder", _settingsOutputFolder, typeof(DefaultAsset), false);
            _prefabOutputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Prefab Folder", _prefabOutputFolder, typeof(DefaultAsset), false);
            _allEnemySettings = (AllEnemySettings)EditorGUILayout.ObjectField("All Enemy Settings", _allEnemySettings, typeof(AllEnemySettings), false);
            _appendToAllEnemySettings = EditorGUILayout.Toggle("Append To List", _appendToAllEnemySettings);

            using (new EditorGUI.DisabledScope(!_appendToAllEnemySettings))
                _replaceExistingTypeInAllEnemySettings = EditorGUILayout.Toggle("Replace Existing Type", _replaceExistingTypeInAllEnemySettings);

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(!CanCreateEnemy()))
            {
                if (GUILayout.Button("Create Enemy"))
                    CreateEnemy();
            }
        }

        private bool CanCreateEnemy()
        {
            return !string.IsNullOrWhiteSpace(_typeId) &&
                   (!_createPrefabFromTemplate || _templateEnemyPrefab != null) &&
                   (_createPrefabFromTemplate || _enemyPrefab != null) &&
                   _pointPrice > 0 &&
                   _maxHealth > 0f &&
                   _maxSpeed >= 0f &&
                   _damage > 0f &&
                   _attackDistance > 0f &&
                   _attackSpeed > 0f &&
                   (!_createLootDefinition || !string.IsNullOrWhiteSpace(_dropItemTypeId));
        }

        private void CreateEnemy()
        {
            var settingsFolder = GetFolderPath(_settingsOutputFolder, DefaultEnemySettingsFolder);
            var prefabFolder = GetFolderPath(_prefabOutputFolder, DefaultEnemyPrefabFolder);
            var safeName = ObjectNames.NicifyVariableName(_typeId).Replace(" ", string.Empty);

            EnsureFolderExists(settingsFolder);

            var prefab = _createPrefabFromTemplate
                ? CreateEnemyPrefab(prefabFolder, safeName)
                : _enemyPrefab;

            var enemySettings = CreateEnemySettings(settingsFolder, safeName, prefab);

            if (_appendToAllEnemySettings && _allEnemySettings != null)
                AppendEnemySettings(enemySettings);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = enemySettings;
            EditorGUIUtility.PingObject(enemySettings);
        }

        private Enemy CreateEnemyPrefab(string prefabFolder, string safeName)
        {
            EnsureFolderExists(prefabFolder);

            var templatePath = AssetDatabase.GetAssetPath(_templateEnemyPrefab);
            var prefabPath = AssetDatabase.GenerateUniqueAssetPath($"{prefabFolder}/{safeName}.prefab");

            if (!AssetDatabase.CopyAsset(templatePath, prefabPath))
                throw new System.InvalidOperationException($"Failed to copy enemy prefab from {templatePath} to {prefabPath}.");

            var prefab = AssetDatabase.LoadAssetAtPath<Enemy>(prefabPath);
            prefab.gameObject.name = safeName;
            EditorUtility.SetDirty(prefab.gameObject);

            return prefab;
        }

        private EnemySettings CreateEnemySettings(string settingsFolder, string safeName, Enemy prefab)
        {
            var enemySettings = CreateInstance<EnemySettings>();
            var path = AssetDatabase.GenerateUniqueAssetPath($"{settingsFolder}/{safeName}.asset");

            AssetDatabase.CreateAsset(enemySettings, path);

            var serialized = new SerializedObject(enemySettings);
            serialized.FindProperty("<TypeId>k__BackingField").stringValue = _typeId.Trim();
            serialized.FindProperty("<EnemyPf>k__BackingField").objectReferenceValue = prefab;
            serialized.FindProperty("<PointPrice>k__BackingField").intValue = Mathf.Max(1, _pointPrice);
            serialized.FindProperty("<MaxHealth>k__BackingField").floatValue = Mathf.Max(1f, _maxHealth);
            serialized.FindProperty("<MaxSpeed>k__BackingField").floatValue = Mathf.Max(0f, _maxSpeed);
            serialized.FindProperty("<Damage>k__BackingField").floatValue = Mathf.Max(0.01f, _damage);
            serialized.FindProperty("<AttackDistance>k__BackingField").floatValue = Mathf.Max(0.01f, _attackDistance);
            serialized.FindProperty("<AttackSpeed>k__BackingField").floatValue = Mathf.Max(0.01f, _attackSpeed);

            var lootDefinitions = serialized.FindProperty("<LootDefinitions>k__BackingField");
            lootDefinitions.arraySize = _createLootDefinition ? 1 : 0;

            if (_createLootDefinition)
            {
                var loot = lootDefinitions.GetArrayElementAtIndex(0);
                loot.FindPropertyRelative("<DropItemTypeId>k__BackingField").stringValue = _dropItemTypeId.Trim();
                loot.FindPropertyRelative("<ChanceToDropItem>k__BackingField").floatValue = Mathf.Clamp01(_chanceToDropItem);
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(enemySettings);

            return enemySettings;
        }

        private void AppendEnemySettings(EnemySettings enemySettings)
        {
            var serialized = new SerializedObject(_allEnemySettings);
            var allSettings = serialized.FindProperty("<AllSettings>k__BackingField");

            for (var i = 0; i < allSettings.arraySize; i++)
            {
                var existing = allSettings.GetArrayElementAtIndex(i).objectReferenceValue as EnemySettings;

                if (existing == enemySettings)
                    return;

                if (existing != null && existing.TypeId == enemySettings.TypeId)
                {
                    if (!_replaceExistingTypeInAllEnemySettings)
                    {
                        Debug.LogWarning($"Enemy type '{enemySettings.TypeId}' already exists in All Enemy Settings.");
                        return;
                    }

                    allSettings.GetArrayElementAtIndex(i).objectReferenceValue = enemySettings;
                    serialized.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(_allEnemySettings);
                    return;
                }
            }

            allSettings.arraySize++;
            allSettings.GetArrayElementAtIndex(allSettings.arraySize - 1).objectReferenceValue = enemySettings;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(_allEnemySettings);
        }

        private static Enemy FindDefaultEnemyPrefab()
        {
            var guids = AssetDatabase.FindAssets("Enemy_Dummy t:Prefab", new[] { DefaultEnemyPrefabFolder });

            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets("t:Prefab", new[] { DefaultEnemyPrefabFolder });

            if (guids.Length == 0)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Enemy>(path);
        }

        private static AllEnemySettings FindDefaultAllEnemySettings()
        {
            var guids = AssetDatabase.FindAssets("t:AllEnemySettings", new[] { DefaultEnemySettingsFolder });

            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets("t:AllEnemySettings");

            if (guids.Length == 0)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AllEnemySettings>(path);
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
