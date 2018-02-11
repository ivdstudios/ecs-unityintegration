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
    [CustomEditor (typeof (EcsWorldObserver))]
    class EcsWorldObserverInspector : Editor {
        public override void OnInspectorGUI () {
            var observer = target as EcsWorldObserver;
            var stats = observer.GetStats ();
            var guiEnabled = GUI.enabled;
            GUI.enabled = true;
            GUILayout.BeginVertical (GUI.skin.box);
            EditorGUILayout.LabelField ("Components", stats.Components.ToString ());
            EditorGUILayout.LabelField ("Filters", stats.Filters.ToString ());
            EditorGUILayout.LabelField ("Active entities", stats.ActiveEntities.ToString ());
            EditorGUILayout.LabelField ("Reserved entities", stats.ReservedEntities.ToString ());
            GUILayout.EndVertical ();
            GUI.enabled = guiEnabled;
        }
    }

    public sealed class EcsWorldObserver : MonoBehaviour, IEcsWorldListener {
        EcsWorld _world;

        readonly Dictionary<int, GameObject> _entities = new Dictionary<int, GameObject> (1024);

        public static GameObject Create (EcsWorld world, string name = null) {
            if (world == null) {
                throw new ArgumentNullException ("world");
            }
            var go = new GameObject (name != null ? string.Format ("[ECS-WORLD {0}]", name) : "[ECS-WORLD]");
            DontDestroyOnLoad (go);
            go.hideFlags = HideFlags.NotEditable;
            var observer = go.AddComponent<EcsWorldObserver> ();
            observer._world = world;
            world.AddEventListener (observer);
            return go;
        }

        public EcsWorldStats GetStats () {
            return _world.GetStats ();
        }

        void IEcsWorldListener.OnEntityCreated (int entity) {
            GameObject go;
            if (!_entities.TryGetValue (entity, out go)) {
                go = new GameObject (entity.ToString ("D8"));
                go.transform.SetParent (transform, false);
                go.hideFlags = HideFlags.NotEditable;
                var unityEntity = go.AddComponent<EcsEntityObserver> ();
                unityEntity.World = _world;
                unityEntity.Id = entity;
                _entities[entity] = go;
            }
            go.SetActive (true);
        }

        void IEcsWorldListener.OnEntityRemoved (int entity) {
            GameObject go;
            if (!_entities.TryGetValue (entity, out go)) {
                throw new Exception ("Unity visualization not exists, looks like a bug");
            }
            go.SetActive (false);
        }

        void OnDestroy () {
            if (_world != null) {
                _world.RemoveEventListener (this);
                _world = null;
            }
        }
    }
}