[![gitter](https://img.shields.io/gitter/room/leopotam/ecs.svg)](https://gitter.im/leopotam/ecs)
[![license](https://img.shields.io/github/license/Leopotam/ecs-ui.svg)](https://github.com/Leopotam/ecs-ui/blob/develop/LICENSE)
# UnityEditor integration for Entity Component System framework
[UnityEditor integration](https://github.com/Leopotam/ecs-unityintegration) for [ECS framework](https://github.com/Leopotam/ecs).

> **This software in work-in-progress stage.**

> Tested / developed on unity 2017.3 and contains assembly definition for compiling to separate assembly file for performance reason.

> Dependent on [ECS framework](https://github.com/Leopotam/ecs) - ECS framework should be imported to unity project first.

# Integration

Integration can be processed with one call of `LeopotamGroup.Ecs.UnityIntegration.EcsWorldObserver.Create()` metod - this call should be wrapped to `#if UNITY_EDITOR` preprocessor define:
```
public class Startup : MonoBehaviour {
    EcsSystems _systems;

    void Start () {
        var world = new EcsWorld ();
        
        #if UNITY_EDITOR
            UnityIntegration.EcsWorldObserver.Create (world);
        #endif
        
        _systems = new EcsSystems(world)
            .Add (new RunSystem1());
            // Additional initialization here...
        _systems.Initialize ();
    }
}
```

# Limitations

## I can't edit component fields at any ecs-entity observer.
By design, observer works as readonly copy of ecs world data.

## I know that data in component updated, but I can't see it in runtime.
By design, inspector will be updated only on any unity editor interaction. For example, you can click inside inspector and component's data will be updated.

# License
The software released under the terms of the MIT license. Enjoy.