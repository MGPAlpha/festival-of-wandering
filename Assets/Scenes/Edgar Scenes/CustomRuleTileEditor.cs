using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace UnityEditor
{
    [CustomEditor(typeof(NewCustomRuleTile))]
    [CanEditMultipleObjects]
    public class CustomRuleTileEditor : RuleTileEditor {
        protected override void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
            {
                RuleTile.TilingRule rule = tile.m_TilingRules[index];
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
                rule.m_ColliderType = (Tile.ColliderType)EditorGUI.EnumPopup(new Rect(inspectorRect.xMin + k_LabelWidth, y, inspectorRect.width - k_LabelWidth, k_SingleLineHeight), rule.m_ColliderType);
                y += k_SingleLineHeight;
                x += k_SingleLineHeight;

                Rect newInspectorRect = new Rect(rect.xMin, yPos + x, rect.width - matrixSize.x - spriteRect.width - 20f, height);

                RuleInspectorOnGUI(newInspectorRect, rule);
                RuleMatrixOnGUI(tile, matrixRect, bounds, rule);
                SpriteOnGUI(spriteRect, rule);
            }
    }
}
