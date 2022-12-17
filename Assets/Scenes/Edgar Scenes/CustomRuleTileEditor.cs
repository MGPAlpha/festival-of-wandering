using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditorInternal;
using UnityEditor;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    [CustomEditor(typeof(TreeRuleTile))]
    [CanEditMultipleObjects]
    public class CustomRuleTileEditor : RuleTileEditor {
        private static readonly string k_UndoName = L10n.Tr("Change RuleTile");

        private static class Styles
        {
            public static readonly GUIContent defaultSprite = EditorGUIUtility.TrTextContent("Default Sprite"
                , "The default Sprite set when creating a new Rule.");
            public static readonly GUIContent defaultGameObject = EditorGUIUtility.TrTextContent("Default GameObject"
                , "The default GameObject set when creating a new Rule.");
            public static readonly GUIContent defaultCollider = EditorGUIUtility.TrTextContent("Default Collider"
                , "The default Collider Type set when creating a new Rule.");

            public static readonly GUIContent emptyRuleTileInfo =
                EditorGUIUtility.TrTextContent(
                    "Drag Sprite or Sprite Texture assets \n" +
                    " to start creating a Rule Tile.");
            
            public static readonly GUIContent extendNeighbor = EditorGUIUtility.TrTextContent("Extend Neighbor"
                , "Enabling this allows you to increase the range of neighbors beyond the 3x3 box.");

            public static readonly GUIContent numberOfTilingRules = EditorGUIUtility.TrTextContent(
                "Number of Tiling Rules"
                , "Change this to adjust of the number of tiling rules.");
            
            public static readonly GUIContent tilingRules = EditorGUIUtility.TrTextContent("Tiling Rules");
            public static readonly GUIContent tilingRulesGameObject = EditorGUIUtility.TrTextContent("GameObject"
                , "The GameObject for the Tile which fits this Rule.");
            public static readonly GUIContent tilingRulesCollider = EditorGUIUtility.TrTextContent("Collider"
                , "The Collider Type for the Tile which fits this Rule");
            public static readonly GUIContent tilingRulesOutput = EditorGUIUtility.TrTextContent("Output"
                , "The Output for the Tile which fits this Rule. Each Output type has its own properties.");

            public static readonly GUIContent tilingRulesNoise = EditorGUIUtility.TrTextContent("Noise"
                , "The Perlin noise factor when placing the Tile.");
            public static readonly GUIContent tilingRulesShuffle = EditorGUIUtility.TrTextContent("Shuffle"
                , "The randomized transform given to the Tile when placing it.");
            public static readonly GUIContent tilingRulesRandomSize = EditorGUIUtility.TrTextContent("Size"
                , "The number of Sprites to randomize from.");

            public static readonly GUIContent tilingRulesMinSpeed = EditorGUIUtility.TrTextContent("Min Speed"
                , "The minimum speed at which the animation is played.");
            public static readonly GUIContent tilingRulesMaxSpeed = EditorGUIUtility.TrTextContent("Max Speed"
                , "The maximum speed at which the animation is played.");
            public static readonly GUIContent tilingRulesAnimationSize = EditorGUIUtility.TrTextContent("Size"
                , "The number of Sprites in the animation.");

            public static readonly GUIStyle extendNeighborsLightStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                fontSize = 10,
                normal = new GUIStyleState()
                {
                    textColor = Color.black
                }
            };
            
            public static readonly GUIStyle extendNeighborsDarkStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                fontSize = 10,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                }
            };
        }
        private List<Sprite> dragAndDropSprites;

        private ReorderableList m_ReorderableList;
        private SerializedProperty m_TilingRules;
        private MethodInfo m_ClearCacheMethod;

        protected override void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
            {
                //TreeRuleTile treeTile = (TreeRuleTile)tile;
                RuleTile.TilingRule rule = tile.m_TilingRules[index];
                //TreeRuleTile.TilingRule treeRule = (TreeRuleTile.TilingRule)treeTile.m_TilingRules[index];

                BoundsInt bounds = GetRuleGUIBounds(rule.GetBounds(), rule);

                float yPos = rect.yMin + 2f;
                float height = rect.height + (2 * k_SingleLineHeight) - k_PaddingBetweenRules;
                Vector2 matrixSize = GetMatrixSize(bounds);

                Rect spriteRect = new Rect(rect.xMax - k_DefaultElementHeight - 5f, yPos, k_DefaultElementHeight, k_DefaultElementHeight);
                Rect matrixRect = new Rect(rect.xMax - matrixSize.x - spriteRect.width - 10f, yPos, matrixSize.x, matrixSize.y);
                Rect inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixSize.x - spriteRect.width - 20f, height);

                float y = inspectorRect.yMin;
                float x = 0;
                GUI.Label(new Rect(inspectorRect.xMin, y, k_LabelWidth, k_SingleLineHeight), "LOL");
                rule.odd = EditorGUI.Toggle(new Rect(inspectorRect.xMin + k_LabelWidth, y, inspectorRect.width - k_LabelWidth, k_SingleLineHeight), rule.odd);
                y += k_SingleLineHeight;
                x += k_SingleLineHeight;

                Rect newInspectorRect = new Rect(rect.xMin, yPos + x, rect.width - matrixSize.x - spriteRect.width - 20f, height);

                RuleInspectorOnGUI(newInspectorRect, rule);
                RuleMatrixOnGUI(tile, matrixRect, bounds, rule);
                SpriteOnGUI(spriteRect, rule);
            }
        
        public override void OnEnable()
        {
            m_ReorderableList = new ReorderableList(tile != null ? tile.m_TilingRules : null, typeof(RuleTile.TilingRule), true, true, true, true);
            m_ReorderableList.drawHeaderCallback = OnDrawHeader;
            m_ReorderableList.drawElementCallback = OnDrawElement;
            m_ReorderableList.elementHeightCallback = GetElementHeight;
            m_ReorderableList.onChangedCallback = ListUpdated;
            m_ReorderableList.onAddDropdownCallback = OnAddDropdownElement;

            // Required to adjust element height changes
            var rolType = GetType("UnityEditorInternal.ReorderableList");
            if (rolType != null)
            {
                // ClearCache was changed to InvalidateCache in newer versions of Unity.
                // To maintain backwards compatibility, we will attempt to retrieve each method in order
                m_ClearCacheMethod = rolType.GetMethod("InvalidateCache", BindingFlags.Instance | BindingFlags.NonPublic);
                if (m_ClearCacheMethod == null)
                    m_ClearCacheMethod = rolType.GetMethod("ClearCache", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            
            m_TilingRules = serializedObject.FindProperty("m_TilingRules");
        }

        private float GetElementHeight(int index)
        {
            RuleTile.TilingRule rule = tile.m_TilingRules[index];
            return GetElementHeight(rule) + 24f;
        }

        private void OnAddDropdownElement(Rect rect, ReorderableList list)
        {
            if (0 <= list.index && list.index < tile.m_TilingRules.Count && list.IsSelected(list.index))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(EditorGUIUtility.TrTextContent("Add"), false, OnAddElement, list);
                menu.AddItem(EditorGUIUtility.TrTextContent("Duplicate"), false, OnDuplicateElement, list);
                menu.DropDown(rect);
            }
            else
            {
                OnAddElement(list);
            }
        }

        private void OnAddElement(object obj)
        {
            var list = obj as ReorderableList;
            RuleTile.TilingRule rule = new RuleTile.TilingRule();
            rule.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Single;
            rule.m_Sprites[0] = tile.m_DefaultSprite;
            rule.m_GameObject = tile.m_DefaultGameObject;
            rule.m_ColliderType = tile.m_DefaultColliderType;

            var count = m_TilingRules.arraySize;
            ResizeRuleTileList(count + 1);
            
            if (list.index == -1  || list.index >= list.count)
                tile.m_TilingRules[count] = rule;
            else
            {
                tile.m_TilingRules.Insert(list.index + 1, rule);
                tile.m_TilingRules.RemoveAt(count + 1);
                if (list.IsSelected(list.index))
                    list.index += 1;
            }
            UpdateTilingRuleIds();
        }

        private void OnDuplicateElement(object obj)
        {
            var list = obj as ReorderableList;
            if (list.index < 0 || list.index >= tile.m_TilingRules.Count)
                return;

            var copyRule = tile.m_TilingRules[list.index];
            var rule = copyRule.Clone();
            
            var count = m_TilingRules.arraySize;
            ResizeRuleTileList(count + 1);
            
            tile.m_TilingRules.Insert(list.index + 1, rule);
            tile.m_TilingRules.RemoveAt(count + 1);
            if (list.IsSelected(list.index))
                list.index += 1;
            UpdateTilingRuleIds();
        }

        private void ResizeRuleTileList(int count)
        {
            if (m_TilingRules.arraySize == count)
                return;

            var isEmpty = m_TilingRules.arraySize == 0;
            m_TilingRules.arraySize = count;
            serializedObject.ApplyModifiedProperties();
            if (isEmpty)
            {
                for (int i = 0; i < count; ++i)
                    tile.m_TilingRules[i] = new RuleTile.TilingRule();
            }
            UpdateTilingRuleIds();
        }

        private void UpdateTilingRuleIds()
        {
            var existingIdSet = new HashSet<int>();
            var usedIdSet = new HashSet<int>();
            foreach (var rule in tile.m_TilingRules)
            {
                existingIdSet.Add(rule.m_Id);
            }
            foreach (var rule in tile.m_TilingRules)
            {
                if (usedIdSet.Contains(rule.m_Id))
                {
                    while (existingIdSet.Contains(rule.m_Id))
                        rule.m_Id++;
                    existingIdSet.Add(rule.m_Id);
                }
                usedIdSet.Add(rule.m_Id);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(target, k_UndoName);

            EditorGUI.BeginChangeCheck();

            tile.m_DefaultSprite = EditorGUILayout.ObjectField(Styles.defaultSprite, tile.m_DefaultSprite, typeof(Sprite), false) as Sprite;
            tile.m_DefaultGameObject = EditorGUILayout.ObjectField(Styles.defaultGameObject, tile.m_DefaultGameObject, typeof(GameObject), false) as GameObject;
            tile.m_DefaultColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup(Styles.defaultCollider, tile.m_DefaultColliderType);

            DrawCustomFields(false);

            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            int count = EditorGUILayout.DelayedIntField(Styles.numberOfTilingRules, tile.m_TilingRules?.Count ?? 0);
            if (count < 0)
                count = 0;
            if (EditorGUI.EndChangeCheck())
                ResizeRuleTileList(count);

            if (count == 0)
            {
                Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 5);
                HandleDragAndDrop(rect);
                EditorGUI.DrawRect(rect, dragAndDropActive && rect.Contains(Event.current.mousePosition) ? Color.white : Color.black);
                var innerRect = new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - 2);
                EditorGUI.DrawRect(innerRect, EditorGUIUtility.isProSkin
                    ? (Color) new Color32 (56, 56, 56, 255)
                    : (Color) new Color32 (194, 194, 194, 255));
                DisplayClipboardText(Styles.emptyRuleTileInfo, rect);
                GUILayout.Space(rect.height);
                EditorGUILayout.Space();
            }

            if (m_ReorderableList != null)
                m_ReorderableList.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
                SaveTile();

            GUILayout.Space(k_DefaultElementHeight);
        }

        private static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
                return type;

            var currentAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    type = assembly.GetType(typeName);
                    if (type != null)
                        return type;
                }
            }
            return null;
        }

        private static List<Sprite> GetSpritesFromTexture(Texture2D texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            List<Sprite> sprites = new List<Sprite>();

            foreach (Object asset in assets)
            {
                if (asset is Sprite)
                {
                    sprites.Add(asset as Sprite);
                }
            }

            return sprites;
        }

        private static List<Sprite> GetValidSingleSprites(Object[] objects)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Object obj in objects)
            {
                if (obj is Sprite sprite)
                {
                    result.Add(sprite);
                }
                else if (obj is Texture2D texture2D)
                {
                    List<Sprite> sprites = GetSpritesFromTexture(texture2D);
                    if (sprites.Count > 0)
                    {
                        result.AddRange(sprites);
                    }
                }
            }
            return result;
        }

        private void HandleDragAndDrop(Rect guiRect)
        {
            if (DragAndDrop.objectReferences.Length == 0 || !guiRect.Contains(Event.current.mousePosition))
                return;

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                {
                    dragAndDropSprites = GetValidSingleSprites(DragAndDrop.objectReferences);
                    if (dragAndDropActive)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Event.current.Use();
                        GUI.changed = true;
                    }
                }
                    break;
                case EventType.DragPerform:
                {
                    if (!dragAndDropActive)
                        return;

                    Undo.RegisterCompleteObjectUndo(tile, "Drag and Drop to Rule Tile");
                    ResizeRuleTileList(dragAndDropSprites.Count);
                    for (int i = 0; i < dragAndDropSprites.Count; ++i)
                    {
                        tile.m_TilingRules[i].m_Sprites[0] = dragAndDropSprites[i];
                    }
                    DragAndDropClear();
                    GUI.changed = true;
                    EditorUtility.SetDirty(tile);
                    GUIUtility.ExitGUI();
                }
                    break;
                case EventType.Repaint:
                    // Handled in Render()
                    break;
            }

            if (Event.current.type == EventType.DragExited ||
                Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                DragAndDropClear();
            }
        }
        
        private void DragAndDropClear()
        {
            dragAndDropSprites = null;
            DragAndDrop.visualMode = DragAndDropVisualMode.None;
            Event.current.Use();
        }

        private void DisplayClipboardText(GUIContent clipboardText, Rect position)
        {
            Color old = GUI.color;
            GUI.color = Color.gray;
            var infoSize = GUI.skin.label.CalcSize(clipboardText);
            Rect rect = new Rect(position.center.x - infoSize.x * .5f
                , position.center.y - infoSize.y * .5f
                , infoSize.x
                , infoSize.y);
            GUI.Label(rect, clipboardText);
            GUI.color = old;
        }

        private bool dragAndDropActive
        {
            get
            {
                return dragAndDropSprites != null
                       && dragAndDropSprites.Count > 0;
            }
        }
    }
}
