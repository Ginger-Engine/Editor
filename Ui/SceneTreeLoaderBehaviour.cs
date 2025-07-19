using Engine.Core.Behaviours;
using Engine.Core.Entities;
using Engine.Core.Scenes;
using Engine.Core.Scenes.Loader;
using System.Collections.Generic;

namespace Ginger.Editor.Ui;

public class SceneTreeLoaderBehaviour : IEntityBehaviour
{
    private readonly SceneLoader _sceneLoader;
    private readonly SceneCreator _sceneCreator;
    private readonly SceneManager _sceneManager;

    public SceneTreeLoaderBehaviour(SceneLoader sceneLoader, SceneCreator sceneCreator, SceneManager sceneManager)
    {
        _sceneLoader = sceneLoader;
        _sceneCreator = sceneCreator;
        _sceneManager = sceneManager;
    }

    public void OnStart(Entity entity)
    {
        // 1. Загрузить сцену (пусть путь будет хардкодом)
        var entityInfo = _sceneLoader.Load("resources/scenes/mainScene.scene.yaml");
        var scene = _sceneCreator.Create(entityInfo);

        // 2. Получить все сущности
        var allEntities = scene.Entities.All.Values;

        // 3. Построить список TreeNode
        var nodes = new List<TreeNode>();
        foreach (var e in allEntities)
        {
            nodes.Add(new TreeNode
            {
                Id = e.Id.ToString(),
                Label = e.Name,
                ParentId = e.Parent?.Id.ToString() ?? "",
                IsExpanded = true
            });
        }

        // 4. Передать в TreeViewComponent текущей панели
        if (entity.TryGetComponent<TreeViewComponent>(out var treeView))
        {
            treeView.Nodes = nodes.ToArray();
            entity.Modify((ref TreeViewComponent t) => t = treeView);
        }
        else
        {
            entity.AddComponent(new TreeViewComponent { Nodes = nodes.ToArray() });
        }
    }
} 