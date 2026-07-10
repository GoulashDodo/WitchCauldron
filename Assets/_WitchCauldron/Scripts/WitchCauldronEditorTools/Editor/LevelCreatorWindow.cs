using Gameplay.Battle.Waves.Enums;
using Gameplay.Battle.Waves.SO;
using Gameplay.Level.SO;
using UnityEditor;
using UnityEngine;

namespace WitchCauldronEditorTools.Editor
{
    public class LevelCreatorWindow : EditorWindow
    {
        private const string DefaultLevelFolder = "Assets/_WitchCauldron/Settings/Data/Game/Gameplay/Level";
        private const string DefaultWaveFolder = "Assets/_WitchCauldron/Settings/Data/Game/Gameplay/Wave";

        [SerializeField] private string _levelId = "level_01";
        [SerializeField] private float _waveStartDelay = 2f;
        [SerializeField] private float _waveStartTime;
        [SerializeField] private float _spawnInterval = 1f;
        [SerializeField] private WaveSpawnMode _spawnMode = WaveSpawnMode.ManualCount;
        [SerializeField] private int _pointBudget = 10;
        [SerializeField] private string _enemyTypeId = "Enemy_Dummy";
        [SerializeField] private int _enemyCount = 3;
        [SerializeField] private int _enemyWeight = 1;
        [SerializeField] private int _enemyMinCount;
        [SerializeField] private int _enemyMaxCount;
        [SerializeField] private WaveType _waveType = WaveType.Normal;
        [SerializeField] private SpawnPositionMode _spawnPositionMode = SpawnPositionMode.RandomInArea;
        [SerializeField] private float _specificSpawnY;
        [SerializeField] private DefaultAsset _levelOutputFolder;
        [SerializeField] private DefaultAsset _waveOutputFolder;
        [SerializeField] private AllLevelSettings _allLevelSettings;
        [SerializeField] private bool _appendToAllLevelSettings = true;

        [MenuItem("Tools/Witch Cauldron/Level Creator")]
        public static void Open()
        {
            GetWindow<LevelCreatorWindow>("Level Creator");
        }

        private void OnEnable()
        {
            _allLevelSettings ??= FindDefaultAllLevelSettings();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);
            _levelId = EditorGUILayout.TextField("Level Id", _levelId);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Wave", EditorStyles.boldLabel);
            _waveStartDelay = EditorGUILayout.FloatField("Start Delay", _waveStartDelay);
            _waveStartTime = EditorGUILayout.FloatField("Wave Delay", _waveStartTime);
            _spawnInterval = EditorGUILayout.FloatField("Spawn Interval", _spawnInterval);
            _waveType = (WaveType)EditorGUILayout.EnumPopup("Wave Type", _waveType);
            _spawnMode = (WaveSpawnMode)EditorGUILayout.EnumPopup("Spawn Mode", _spawnMode);

            using (new EditorGUI.DisabledScope(_spawnMode != WaveSpawnMode.PointBudget))
                _pointBudget = EditorGUILayout.IntField("Point Budget", _pointBudget);

            _spawnPositionMode = (SpawnPositionMode)EditorGUILayout.EnumPopup("Spawn Position Mode", _spawnPositionMode);

            using (new EditorGUI.DisabledScope(_spawnPositionMode != SpawnPositionMode.SpecificPosition))
                _specificSpawnY = EditorGUILayout.FloatField("Specific Y Position", _specificSpawnY);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Enemy", EditorStyles.boldLabel);
            _enemyTypeId = EditorGUILayout.TextField("Enemy Type Id", _enemyTypeId);
            _enemyWeight = EditorGUILayout.IntField("Enemy Weight", _enemyWeight);

            using (new EditorGUI.DisabledScope(_spawnMode != WaveSpawnMode.ManualCount))
                _enemyCount = EditorGUILayout.IntField("Manual Count", _enemyCount);

            using (new EditorGUI.DisabledScope(_spawnMode != WaveSpawnMode.PointBudget))
            {
                _enemyMinCount = EditorGUILayout.IntField("Min Count", _enemyMinCount);
                _enemyMaxCount = EditorGUILayout.IntField("Max Count", _enemyMaxCount);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            _levelOutputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Level Folder", _levelOutputFolder, typeof(DefaultAsset), false);
            _waveOutputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Wave Folder", _waveOutputFolder, typeof(DefaultAsset), false);
            _allLevelSettings = (AllLevelSettings)EditorGUILayout.ObjectField("All Level Settings", _allLevelSettings, typeof(AllLevelSettings), false);
            _appendToAllLevelSettings = EditorGUILayout.Toggle("Append To List", _appendToAllLevelSettings);

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(!CanCreateLevel()))
            {
                if (GUILayout.Button("Create Level"))
                    CreateLevel();
            }
        }

        private bool CanCreateLevel()
        {
            return !string.IsNullOrWhiteSpace(_levelId) &&
                   !string.IsNullOrWhiteSpace(_enemyTypeId) &&
                   _spawnInterval > 0f &&
                   (_spawnMode != WaveSpawnMode.ManualCount || _enemyCount > 0) &&
                   (_spawnMode != WaveSpawnMode.PointBudget || _pointBudget > 0) &&
                   _enemyWeight > 0;
        }

        private void CreateLevel()
        {
            var levelFolder = GetFolderPath(_levelOutputFolder, DefaultLevelFolder);
            var waveFolder = GetFolderPath(_waveOutputFolder, DefaultWaveFolder);

            EnsureFolderExists(levelFolder);
            EnsureFolderExists(waveFolder);

            var safeName = ObjectNames.NicifyVariableName(_levelId).Replace(" ", string.Empty);
            var waveSettings = CreateWaveSettings(waveFolder, safeName);
            var levelSettings = CreateLevelSettings(levelFolder, safeName, waveSettings);

            if (_appendToAllLevelSettings && _allLevelSettings != null)
                AppendLevelSettings(levelSettings);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = levelSettings;
            EditorGUIUtility.PingObject(levelSettings);
        }

        private WaveSettings CreateWaveSettings(string waveFolder, string safeName)
        {
            var waveSettings = CreateInstance<WaveSettings>();
            var path = AssetDatabase.GenerateUniqueAssetPath($"{waveFolder}/{safeName}_WaveSettings.asset");

            AssetDatabase.CreateAsset(waveSettings, path);

            var serialized = new SerializedObject(waveSettings);
            serialized.FindProperty("<StartDelay>k__BackingField").floatValue = Mathf.Max(0f, _waveStartDelay);

            var waves = serialized.FindProperty("<Waves>k__BackingField");
            waves.arraySize = 1;

            var wave = waves.GetArrayElementAtIndex(0);
            wave.FindPropertyRelative("<Type>k__BackingField").enumValueIndex = (int)_waveType;
            wave.FindPropertyRelative("<StartTime>k__BackingField").floatValue = Mathf.Max(0f, _waveStartTime);
            wave.FindPropertyRelative("<SpawnInterval>k__BackingField").floatValue = Mathf.Max(0.1f, _spawnInterval);
            wave.FindPropertyRelative("<SpawnMode>k__BackingField").enumValueIndex = (int)_spawnMode;
            wave.FindPropertyRelative("<PointBudget>k__BackingField").intValue = Mathf.Max(0, _pointBudget);
            wave.FindPropertyRelative("<SpawnPositionMode>k__BackingField").enumValueIndex = (int)_spawnPositionMode;
            wave.FindPropertyRelative("<SpecificSpawnY>k__BackingField").floatValue = _specificSpawnY;

            var enemies = wave.FindPropertyRelative("<Enemies>k__BackingField");
            enemies.arraySize = 1;

            var enemy = enemies.GetArrayElementAtIndex(0);
            enemy.FindPropertyRelative("<EnemyTypeId>k__BackingField").stringValue = _enemyTypeId;
            enemy.FindPropertyRelative("<Count>k__BackingField").intValue = Mathf.Max(1, _enemyCount);
            enemy.FindPropertyRelative("<Weight>k__BackingField").intValue = Mathf.Max(1, _enemyWeight);
            enemy.FindPropertyRelative("<MinCount>k__BackingField").intValue = Mathf.Max(0, _enemyMinCount);
            enemy.FindPropertyRelative("<MaxCount>k__BackingField").intValue = Mathf.Max(0, _enemyMaxCount);

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(waveSettings);

            return waveSettings;
        }

        private LevelSettings CreateLevelSettings(string levelFolder, string safeName, WaveSettings waveSettings)
        {
            var levelSettings = CreateInstance<LevelSettings>();
            var path = AssetDatabase.GenerateUniqueAssetPath($"{levelFolder}/{safeName}_LevelSettings.asset");

            AssetDatabase.CreateAsset(levelSettings, path);

            var serialized = new SerializedObject(levelSettings);
            serialized.FindProperty("<LevelId>k__BackingField").stringValue = _levelId.Trim();
            serialized.FindProperty("<WaveSettings>k__BackingField").objectReferenceValue = waveSettings;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(levelSettings);
            return levelSettings;
        }

        private void AppendLevelSettings(LevelSettings levelSettings)
        {
            var serialized = new SerializedObject(_allLevelSettings);
            var allSettings = serialized.FindProperty("<AllSettings>k__BackingField");

            for (var i = 0; i < allSettings.arraySize; i++)
            {
                if (allSettings.GetArrayElementAtIndex(i).objectReferenceValue == levelSettings)
                    return;
            }

            allSettings.arraySize++;
            allSettings.GetArrayElementAtIndex(allSettings.arraySize - 1).objectReferenceValue = levelSettings;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(_allLevelSettings);
        }

        private static AllLevelSettings FindDefaultAllLevelSettings()
        {
            var guids = AssetDatabase.FindAssets("t:AllLevelSettings", new[] { DefaultLevelFolder });

            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets("t:AllLevelSettings");

            if (guids.Length == 0)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AllLevelSettings>(path);
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
