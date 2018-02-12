// ----------------------------------------------------------------------------
// The MIT License
// UnityEditor integration https://github.com/Leopotam/ecs-unityintegration
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeopotamGroup.Ecs.UnityIntegration {
    public sealed class EcsEntityObserver : MonoBehaviour {
        public EcsWorld World;

        public int Id;
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
#endif