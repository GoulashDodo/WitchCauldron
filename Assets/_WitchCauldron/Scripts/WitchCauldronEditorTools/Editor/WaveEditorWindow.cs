using System.Collections.Generic;
using Gameplay.Battle.BattleEntities.Enemies.SO;
using Gameplay.Battle.Waves.Enums;
using Gameplay.Battle.Waves.SO;
using UnityEditor;
using UnityEngine;

namespace WitchCauldronEditorTools.Editor
{
    public sealed class WaveEditorWindow : EditorWindow
    {
        private const string DefaultWaveFolder = "Assets/_WitchCauldron/Settings/Data/Game/2.Gameplay/Wave";

        private static readonly string StartDelayProperty = "<StartDelay>k__BackingField";
        private static readonly string WavesProperty = "<Waves>k__BackingField";
        private static readonly string TypeProperty = "<Type>k__BackingField";
        private static readonly string StartTimeProperty = "<StartTime>k__BackingField";
        private static readonly string SpawnIntervalProperty = "<SpawnInterval>k__BackingField";
        private static readonly string SpawnModeProperty = "<SpawnMode>k__BackingField";
        private static readonly string PointBudgetProperty = "<PointBudget>k__BackingField";
        private static readonly string SpawnPositionModeProperty = "<SpawnPositionMode>k__BackingField";
        private static readonly string SpecificSpawnPositionProperty = "<SpecificSpawnPosition>k__BackingField";
        private static readonly string EnemiesProperty = "<Enemies>k__BackingField";
        private static readonly string EnemyTypeIdProperty = "<EnemyTypeId>k__BackingField";
        private static readonly string CountProperty = "<Count>k__BackingField";
        private static readonly string WeightProperty = "<Weight>k__BackingField";
        private static readonly string MinCountProperty = "<MinCount>k__BackingField";
        private static readonly string MaxCountProperty = "<MaxCount>k__BackingField";

        [SerializeField] private WaveSettings _waveSettings;
        [SerializeField] private DefaultAsset _outputFolder;
        [SerializeField] private string _newAssetName = "Level_New_WaveSettings";

        private readonly Dictionary<int, bool> _expandedWaves = new();
        private Vector2 _scroll;
        private string[] _enemyTypeIds = new string[0];
        private int _selectedWaveIndex = -1;

        [MenuItem("Tools/Witch Cauldron/Wave Editor")]
        public static void Open()
        {
            GetWindow<WaveEditorWindow>("Wave Editor");
        }

        private void OnEnable()
        {
            RefreshEnemyTypeIds();
        }

        private void OnFocus()
        {
            RefreshEnemyTypeIds();
        }

        private void OnGUI()
        {
            DrawAssetControls();

            if (_waveSettings == null)
                return;

            var serialized = new SerializedObject(_waveSettings);
            var startDelay = serialized.FindProperty(StartDelayProperty);
            var waves = serialized.FindProperty(WavesProperty);

            if (startDelay == null || waves == null)
            {
                EditorGUILayout.HelpBox("WaveSettings has unexpected serialized field names.", MessageType.Error);
                return;
            }

            serialized.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);
            startDelay.floatValue = Mathf.Max(0f, EditorGUILayout.FloatField("Level Start Delay", startDelay.floatValue));

            DrawWaveToolbar(serialized, waves);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            for (var i = 0; i < waves.arraySize; i++)
                DrawWave(serialized, waves, i);

            EditorGUILayout.EndScrollView();

            serialized.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(_waveSettings);
        }

        private void DrawAssetControls()
        {
            EditorGUILayout.LabelField("Asset", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                _waveSettings = (WaveSettings)EditorGUILayout.ObjectField("Wave Settings", _waveSettings, typeof(WaveSettings), false);

                if (GUILayout.Button("Find", GUILayout.Width(56)))
                    _waveSettings = FindDefaultWaveSettings();

                if (GUILayout.Button("Ping", GUILayout.Width(56)) && _waveSettings != null)
                    EditorGUIUtility.PingObject(_waveSettings);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                _newAssetName = EditorGUILayout.TextField("New Asset Name", _newAssetName);

                if (GUILayout.Button("Create", GUILayout.Width(70)))
                    CreateWaveSettingsAsset();
            }

            _outputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Output Folder", _outputFolder, typeof(DefaultAsset), false);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh Enemy Ids"))
                    RefreshEnemyTypeIds();

                EditorGUILayout.LabelField($"{_enemyTypeIds.Length} enemy ids found", EditorStyles.miniLabel, GUILayout.Width(120));
            }
        }

        private void DrawWaveToolbar(SerializedObject serialized, SerializedProperty waves)
        {
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                EditorGUILayout.LabelField($"Waves: {waves.arraySize}", EditorStyles.boldLabel);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add Wave", EditorStyles.toolbarButton, GUILayout.Width(90)))
                {
                    AddWave(waves);
                    serialized.ApplyModifiedProperties();
                    EditorUtility.SetDirty(_waveSettings);
                }
            }
        }

        private void DrawWave(SerializedObject serialized, SerializedProperty waves, int index)
        {
            var wave = waves.GetArrayElementAtIndex(index);
            var enemies = wave.FindPropertyRelative(EnemiesProperty);
            var isExpanded = IsWaveExpanded(index);
            var totalCount = GetTotalEnemyCount(enemies);
            var type = (WaveType)wave.FindPropertyRelative(TypeProperty).enumValueIndex;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _expandedWaves[index] = EditorGUILayout.Foldout(
                        isExpanded,
                        $"Wave {index + 1}: {type} | enemies: {totalCount}",
                        true
                    );

                    GUI.enabled = index > 0;
                    if (GUILayout.Button("Up", GUILayout.Width(38)))
                    {
                        waves.MoveArrayElement(index, index - 1);
                        _selectedWaveIndex = index - 1;
                        GUI.enabled = true;
                        return;
                    }

                    GUI.enabled = index < waves.arraySize - 1;
                    if (GUILayout.Button("Down", GUILayout.Width(52)))
                    {
                        waves.MoveArrayElement(index, index + 1);
                        _selectedWaveIndex = index + 1;
                        GUI.enabled = true;
                        return;
                    }

                    GUI.enabled = true;

                    if (GUILayout.Button("Duplicate", GUILayout.Width(76)))
                    {
                        waves.InsertArrayElementAtIndex(index);
                        _expandedWaves[index + 1] = true;
                        _selectedWaveIndex = index + 1;
                        return;
                    }

                    if (GUILayout.Button("Delete", GUILayout.Width(58)))
                    {
                        if (EditorUtility.DisplayDialog("Delete Wave", $"Delete wave {index + 1}?", "Delete", "Cancel"))
                        {
                            waves.DeleteArrayElementAtIndex(index);
                            _selectedWaveIndex = Mathf.Clamp(_selectedWaveIndex, -1, waves.arraySize - 1);
                        }

                        return;
                    }
                }

                if (!_expandedWaves[index])
                    return;

                _selectedWaveIndex = index;
                DrawWaveFields(wave);
                DrawEnemies(serialized, enemies, wave.FindPropertyRelative(SpawnModeProperty).enumValueIndex);
            }
        }

        private void DrawWaveFields(SerializedProperty wave)
        {
            EditorGUILayout.Space(2f);
            EditorGUILayout.PropertyField(wave.FindPropertyRelative(TypeProperty), new GUIContent("Type"));
            wave.FindPropertyRelative(StartTimeProperty).floatValue = Mathf.Max(
                0f,
                EditorGUILayout.FloatField("Start Time / Delay", wave.FindPropertyRelative(StartTimeProperty).floatValue)
            );
            wave.FindPropertyRelative(SpawnIntervalProperty).floatValue = Mathf.Max(
                0.1f,
                EditorGUILayout.FloatField("Spawn Interval", wave.FindPropertyRelative(SpawnIntervalProperty).floatValue)
            );

            var spawnMode = wave.FindPropertyRelative(SpawnModeProperty);
            EditorGUILayout.PropertyField(spawnMode, new GUIContent("Spawn Mode"));

            using (new EditorGUI.DisabledScope(spawnMode.enumValueIndex != (int)WaveSpawnMode.PointBudget))
            {
                var pointBudget = wave.FindPropertyRelative(PointBudgetProperty);
                pointBudget.intValue = Mathf.Max(0, EditorGUILayout.IntField("Point Budget", pointBudget.intValue));
            }

            var spawnPositionMode = wave.FindPropertyRelative(SpawnPositionModeProperty);
            EditorGUILayout.PropertyField(spawnPositionMode, new GUIContent("Spawn Position"));

            using (new EditorGUI.DisabledScope(spawnPositionMode.enumValueIndex != (int)SpawnPositionMode.SpecificPosition))
                EditorGUILayout.PropertyField(wave.FindPropertyRelative(SpecificSpawnPositionProperty), new GUIContent("Specific Position"));
        }

        private void DrawEnemies(SerializedObject serialized, SerializedProperty enemies, int spawnMode)
        {
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Enemies", EditorStyles.boldLabel);

                if (GUILayout.Button("Add Enemy", GUILayout.Width(90)))
                {
                    AddEnemy(enemies);
                    serialized.ApplyModifiedProperties();
                    EditorUtility.SetDirty(_waveSettings);
                }
            }

            if (enemies.arraySize == 0)
                EditorGUILayout.HelpBox("This wave has no enemies.", MessageType.Warning);

            for (var i = 0; i < enemies.arraySize; i++)
                DrawEnemy(enemies, i, spawnMode);
        }

        private void DrawEnemy(SerializedProperty enemies, int index, int spawnMode)
        {
            var enemy = enemies.GetArrayElementAtIndex(index);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField($"Enemy {index + 1}", EditorStyles.boldLabel);

                    GUI.enabled = index > 0;
                    if (GUILayout.Button("Up", GUILayout.Width(38)))
                    {
                        enemies.MoveArrayElement(index, index - 1);
                        GUI.enabled = true;
                        return;
                    }

                    GUI.enabled = index < enemies.arraySize - 1;
                    if (GUILayout.Button("Down", GUILayout.Width(52)))
                    {
                        enemies.MoveArrayElement(index, index + 1);
                        GUI.enabled = true;
                        return;
                    }

                    GUI.enabled = true;

                    if (GUILayout.Button("Delete", GUILayout.Width(58)))
                    {
                        enemies.DeleteArrayElementAtIndex(index);
                        return;
                    }
                }

                DrawEnemyTypeId(enemy.FindPropertyRelative(EnemyTypeIdProperty));

                using (new EditorGUI.DisabledScope(spawnMode != (int)WaveSpawnMode.ManualCount))
                {
                    var count = enemy.FindPropertyRelative(CountProperty);
                    count.intValue = Mathf.Max(0, EditorGUILayout.IntField("Manual Count", count.intValue));
                }

                var weight = enemy.FindPropertyRelative(WeightProperty);
                weight.intValue = Mathf.Max(1, EditorGUILayout.IntField("Selection Weight", weight.intValue));

                using (new EditorGUI.DisabledScope(spawnMode != (int)WaveSpawnMode.PointBudget))
                {
                    var minCount = enemy.FindPropertyRelative(MinCountProperty);
                    var maxCount = enemy.FindPropertyRelative(MaxCountProperty);

                    minCount.intValue = Mathf.Max(0, EditorGUILayout.IntField("Min Count", minCount.intValue));
                    maxCount.intValue = Mathf.Max(0, EditorGUILayout.IntField("Max Count", maxCount.intValue));

                    if (maxCount.intValue > 0 && minCount.intValue > maxCount.intValue)
                        EditorGUILayout.HelpBox("Min Count is greater than Max Count.", MessageType.Warning);
                }
            }
        }

        private void DrawEnemyTypeId(SerializedProperty typeId)
        {
            if (_enemyTypeIds.Length == 0)
            {
                typeId.stringValue = EditorGUILayout.TextField("Enemy Type Id", typeId.stringValue);
                return;
            }

            var currentIndex = System.Array.IndexOf(_enemyTypeIds, typeId.stringValue);
            var options = new string[_enemyTypeIds.Length + 1];
            options[0] = "Manual...";
            _enemyTypeIds.CopyTo(options, 1);

            var selected = currentIndex >= 0 ? currentIndex + 1 : 0;
            var next = EditorGUILayout.Popup("Enemy Type Id", selected, options);

            if (next > 0)
                typeId.stringValue = _enemyTypeIds[next - 1];
            else
                typeId.stringValue = EditorGUILayout.TextField("Manual Id", typeId.stringValue);
        }

        private void AddWave(SerializedProperty waves)
        {
            var index = waves.arraySize;
            waves.arraySize++;

            var wave = waves.GetArrayElementAtIndex(index);
            SetWaveDefaults(wave);

            _expandedWaves[index] = true;
            _selectedWaveIndex = index;
        }

        private void AddEnemy(SerializedProperty enemies)
        {
            var index = enemies.arraySize;
            enemies.arraySize++;
            SetEnemyDefaults(enemies.GetArrayElementAtIndex(index));
        }

        private void SetWaveDefaults(SerializedProperty wave)
        {
            wave.FindPropertyRelative(TypeProperty).enumValueIndex = (int)WaveType.Normal;
            wave.FindPropertyRelative(StartTimeProperty).floatValue = 0f;
            wave.FindPropertyRelative(SpawnIntervalProperty).floatValue = 1f;
            wave.FindPropertyRelative(SpawnModeProperty).enumValueIndex = (int)WaveSpawnMode.ManualCount;
            wave.FindPropertyRelative(PointBudgetProperty).intValue = 10;
            wave.FindPropertyRelative(SpawnPositionModeProperty).enumValueIndex = (int)SpawnPositionMode.RandomInArea;
            wave.FindPropertyRelative(SpecificSpawnPositionProperty).vector3Value = Vector3.zero;

            var enemies = wave.FindPropertyRelative(EnemiesProperty);
            enemies.arraySize = 1;
            SetEnemyDefaults(enemies.GetArrayElementAtIndex(0));
        }

        private void SetEnemyDefaults(SerializedProperty enemy)
        {
            enemy.FindPropertyRelative(EnemyTypeIdProperty).stringValue = _enemyTypeIds.Length > 0 ? _enemyTypeIds[0] : "Enemy_Dummy";
            enemy.FindPropertyRelative(CountProperty).intValue = 1;
            enemy.FindPropertyRelative(WeightProperty).intValue = 1;
            enemy.FindPropertyRelative(MinCountProperty).intValue = 0;
            enemy.FindPropertyRelative(MaxCountProperty).intValue = 0;
        }

        private bool IsWaveExpanded(int index)
        {
            if (_expandedWaves.TryGetValue(index, out var expanded))
                return expanded;

            var isSelected = index == _selectedWaveIndex;
            _expandedWaves[index] = isSelected;
            return isSelected;
        }

        private static int GetTotalEnemyCount(SerializedProperty enemies)
        {
            if (enemies == null)
                return 0;

            var total = 0;

            for (var i = 0; i < enemies.arraySize; i++)
                total += Mathf.Max(0, enemies.GetArrayElementAtIndex(i).FindPropertyRelative(CountProperty).intValue);

            return total;
        }

        private void CreateWaveSettingsAsset()
        {
            var folder = GetFolderPath(_outputFolder, DefaultWaveFolder);
            EnsureFolderExists(folder);

            var safeName = string.IsNullOrWhiteSpace(_newAssetName)
                ? "WaveSettings"
                : ObjectNames.NicifyVariableName(_newAssetName.Trim()).Replace(" ", string.Empty);

            var waveSettings = CreateInstance<WaveSettings>();
            var path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{safeName}.asset");

            AssetDatabase.CreateAsset(waveSettings, path);

            var serialized = new SerializedObject(waveSettings);
            serialized.FindProperty(StartDelayProperty).floatValue = 2f;
            var waves = serialized.FindProperty(WavesProperty);
            waves.arraySize = 1;
            SetWaveDefaults(waves.GetArrayElementAtIndex(0));
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(waveSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _waveSettings = waveSettings;
            _selectedWaveIndex = 0;
            _expandedWaves.Clear();
            _expandedWaves[0] = true;
            Selection.activeObject = waveSettings;
            EditorGUIUtility.PingObject(waveSettings);
        }

        private void RefreshEnemyTypeIds()
        {
            var guids = AssetDatabase.FindAssets("t:EnemySettings");
            var ids = new List<string>();

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var settings = AssetDatabase.LoadAssetAtPath<EnemySettings>(path);

                if (settings != null && !string.IsNullOrWhiteSpace(settings.TypeId) && !ids.Contains(settings.TypeId))
                    ids.Add(settings.TypeId);
            }

            ids.Sort();
            _enemyTypeIds = ids.ToArray();
        }

        private static WaveSettings FindDefaultWaveSettings()
        {
            var guids = AssetDatabase.FindAssets("t:WaveSettings", new[] { DefaultWaveFolder });

            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets("t:WaveSettings");

            if (guids.Length == 0)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<WaveSettings>(path);
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
