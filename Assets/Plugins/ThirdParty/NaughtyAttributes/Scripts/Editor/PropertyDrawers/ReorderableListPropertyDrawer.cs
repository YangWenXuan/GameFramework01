using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace NaughtyAttributes.Editor
{
    [PropertyDrawer(typeof(ReorderableListAttribute))]
    public class ReorderableListPropertyDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> reorderableListsByPropertyName = new Dictionary<string, ReorderableList>();
        

        public override void DrawProperty(SerializedProperty property)
        {
            EditorDrawUtility.DrawHeader(property);

            if (property.isArray)
            {
                if (!this.reorderableListsByPropertyName.ContainsKey(property.name))
                {
                    ReorderableList reorderableList = new ReorderableList(property.serializedObject, property, true, true, true, true)
                    {
                        drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                        {
                            var element = property.GetArrayElementAtIndex(index);
                            rect.y += 2f;
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
                        }
                    };

                    reorderableList.drawHeaderCallback = (Rect rect) => {
                        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, string.Format("{0} [{1}]", property.displayName, property.arraySize));
                        Rect rect2 = rect;
                        if (reorderableList.count == 0) {
                            rect2.height = (rect2.height * 3f);
                        }
                        Event current = Event.current;
                        EventType type = current.type;
                        if (type != EventType.DragExited) {
                            if (type == EventType.DragUpdated || type == EventType.DragPerform) {
                                if (rect2.Contains(current.mousePosition)) {
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                    if (current.type == EventType.DragPerform) {
                                        DragAndDrop.AcceptDrag();
                                        Object[] objectReferences = DragAndDrop.objectReferences;
                                        for (int i = 0; i < objectReferences.Length; i++) {
                                            Object obj = objectReferences[i];
                                            AddElement(property, obj);
                                        }
                                    }
                                }
                            }
                        } else {
                            DragAndDrop.PrepareStartDrag();
                        }
//                            EditorGUI.LabelField(rect, string.Format("{0}: {1}", property.displayName, property.arraySize), EditorStyles.label);
                    };

                    this.reorderableListsByPropertyName[property.name] = reorderableList;
                }
                if (property.isExpanded) {
                    this.reorderableListsByPropertyName[property.name].DoLayoutList();
                } else {
                    property.isExpanded = (EditorGUILayout.Foldout(property.isExpanded, new GUIContent(string.Format("{0} [{1}]", property.displayName, property.arraySize), property.tooltip)));
                }
            }
            else
            {
                string warning = typeof(ReorderableListAttribute).Name + " can be used only on arrays or lists";
                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, logToConsole: true, context: PropertyUtility.GetTargetObject(property));

                EditorDrawUtility.DrawPropertyField(property);
            }
        }
        
        private static void AddElement(SerializedProperty property, Object obj) {
            property.InsertArrayElementAtIndex(property.arraySize);
            SerializedProperty arrayElementAtIndex = property.GetArrayElementAtIndex(property.arraySize - 1);
            arrayElementAtIndex.objectReferenceValue = obj;
        }

        public override void ClearCache()
        {
            this.reorderableListsByPropertyName.Clear();
        }
    }
}
