// ----------------------------------------------------------------------------
// The MIT License
// UnityEditor integration https://github.com/Leopotam/ecs-unityintegration
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.Ecs.UnityIntegration {
    [CustomEditor (typeof (EcsSystemsObserver))]
    class EcsSystemsObserverInspector : Editor {
        static readonly List<IEcsPreInitSystem> _preInitList = new List<IEcsPreInitSystem> ();

        static readonly List<IEcsInitSystem> _initList = new List<IEcsInitSystem> ();

        static readonly List<IEcsRunSystem> _runList = new List<IEcsRunSystem> ();

        public override void OnInspectorGUI () {
            var observer = target as EcsSystemsObserver;
            var systems = observer.GetSystems ();
            var guiEnabled = GUI.enabled;
            GUI.enabled = true;
            systems.IsActive = EditorGUILayout.Toggle ("Run systems activated", systems.IsActive);

            systems.GetPreInitSystems (_preInitList);
            if (_preInitList.Count > 0) {
                GUILayout.BeginVertical (GUI.skin.box);
                EditorGUILayout.LabelField ("PreInitialize systems", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (var system in _preInitList) {
                    EditorGUILayout.LabelField (system.GetType ().Name);
                }
                EditorGUI.indentLevel--;
                GUILayout.EndVertical ();
                _preInitList.Clear ();
            }

            systems.GetInitSystems (_initList);
            if (_initList.Count > 0) {
                GUILayout.BeginVertical (GUI.skin.box);
                EditorGUILayout.LabelField ("Initialize systems", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (var system in _initList) {
                    EditorGUILayout.LabelField (system.GetType ().Name);
                }
                EditorGUI.indentLevel--;
                GUILayout.EndVertical ();
                _initList.Clear ();
            }

            if (!systems.IsActive) {
                GUI.enabled = false;
            }
            DrawRunSystems ("Update systems", systems, EcsRunSystemType.Update);
            DrawRunSystems ("LateUpdate systems", systems, EcsRunSystemType.LateUpdate);
            DrawRunSystems ("FixedUpdate systems", systems, EcsRunSystemType.FixedUpdate);
            GUI.enabled = guiEnabled;
        }

        void DrawRunSystems (string title, EcsSystems systems, EcsRunSystemType runSystemType) {
            systems.GetRunSystems (runSystemType, _runList);
            if (_runList.Count > 0) {
                GUILayout.BeginVertical (GUI.skin.box);
                EditorGUILayout.LabelField (title, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (var system in _runList) {
                    EditorGUILayout.LabelField (system.GetType ().Name);
                }
                EditorGUI.indentLevel--;
                GUILayout.EndVertical ();
                _runList.Clear ();
            }
        }
    }

    public sealed class EcsSystemsObserver : MonoBehaviour, IEcsSystemsListener {
        EcsSystems _systems;

        public static GameObject Create (EcsSystems systems, string name = null) {
            if (systems == null) {
                throw new ArgumentNullException ("systems");
            }
            var go = new GameObject (name != null ? string.Format ("[ECS-SYSTEMS {0}]", name) : "[ECS-SYSTEMS]");
            DontDestroyOnLoad (go);
            go.hideFlags = HideFlags.NotEditable;
            var observer = go.AddComponent<EcsSystemsObserver> ();
            observer._systems = systems;
            systems.AddEventListener (observer);
            return go;
        }

        public EcsSystems GetSystems () {
            return _systems;
        }

        void OnDestroy () {
            if (_systems != null) {
                _systems.RemoveEventListener (this);
                _systems = null;
            }
        }

        void IEcsSystemsListener.OnSystemsDestroyed () {
            OnDestroy ();
        }
    }
}