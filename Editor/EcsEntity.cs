// ----------------------------------------------------------------------------
// The MIT License
// UnityEditor integration https://github.com/Leopotam/ecs-unityintegration
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.Ecs.UnityIntegration {
    [CustomEditor (typeof (EcsEntityObserver))]
    class EcsEntityObserverInspector : Editor {
        static List<object> _componentsCache = new List<object> (6);

        EcsEntityObserver _entity;

        public override void OnInspectorGUI () {
            if (_entity.World == null) { return; }
            _entity.World.GetComponents (_entity.Id, _componentsCache);
            var guiEnabled = GUI.enabled;
            GUI.enabled = true;
            DrawComponents (_componentsCache);
            GUI.enabled = guiEnabled;
        }

        void OnEnable () {
            _entity = target as EcsEntityObserver;
        }

        void OnDisable () {
            _componentsCache.Clear ();
            _entity = null;
        }

        void DrawComponents (List<object> componentsCache) {
            foreach (var component in componentsCache) {
                var type = component.GetType ();
                GUILayout.BeginVertical (GUI.skin.box);
                EditorGUILayout.LabelField (type.Name, EditorStyles.boldLabel);
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                foreach (var field in type.GetFields (BindingFlags.Instance | BindingFlags.Public)) {
                    DrawTypeField (component, field);
                }
                EditorGUI.indentLevel = indent;
                GUILayout.EndVertical ();
                // EditorGUILayout.Separator ();
                EditorGUILayout.Space ();
            }
        }

        void DrawTypeField (object instance, FieldInfo field) {
            var fieldValue = field.GetValue (instance);
            var fieldType = fieldValue == null ? field.FieldType : fieldValue.GetType ();
            if (fieldType == typeof (UnityEngine.Object) || fieldType.IsSubclassOf (typeof (UnityEngine.Object))) {
                GUILayout.BeginHorizontal ();
                EditorGUILayout.LabelField (field.Name, GUILayout.MaxWidth (EditorGUIUtility.labelWidth - 16));
                var guiEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUILayout.ObjectField (fieldValue as UnityEngine.Object, fieldType, false);
                GUI.enabled = guiEnabled;
                GUILayout.EndHorizontal ();
                return;
            }
            EditorGUILayout.LabelField (field.Name, fieldValue != null ? fieldValue.ToString () : "null");
        }
    }

    sealed class EcsEntityObserver : MonoBehaviour {
        public EcsWorld World;

        public int Id;
    }
}