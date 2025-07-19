using Engine.Core.Entities;
using Engine.Core.Scenes.Loader;
using System.Collections.Generic;
using System.Linq;
using Engine.Core;

namespace Ginger.Editor.Ui;

public class PrefabInstantiator
{
    private readonly SceneLoader _loader;
    private readonly SceneCreator _creator;

    public PrefabInstantiator(SceneLoader loader, SceneCreator creator)
    {
        _loader = loader;
        _creator = creator;
    }

    // 1. Загрузка шаблона с параметрами
    public Entity Load(string prefabPath, Dictionary<string, object>? parameters = null)
    {
        var entityInfo = _loader.Load(prefabPath, parameters);
        var entity = _creator.CreateEntity(entityInfo);
        return entity;
    }

    // 2. Быстрое клонирование с возможностью оверрайда компонентов
    public Entity Instantiate(Entity template, IEnumerable<IComponent> overrideComponents = null)
    {
        var clone = DeepCloneEntity(template);
        if (overrideComponents != null)
        {
            foreach (var component in overrideComponents)
            {
                clone.AddOrApplyComponent(component);
            }
        }
        return clone;
    }

    private Entity DeepCloneEntity(Entity original)
    {
        var clone = new Entity
        {
            Name = original.Name,
            IsEnabled = original.IsEnabled
        };
        
        foreach (var (type, component) in original.Components)
        {
            if (component is not IComponent) continue;
            clone.AddOrApplyComponent(component);
        }
        
        foreach (var child in original.Children)
        {
            var childClone = DeepCloneEntity(child);
            childClone.Parent = clone;
            clone.Children.Add(childClone);
        }
        return clone;
    }
} 