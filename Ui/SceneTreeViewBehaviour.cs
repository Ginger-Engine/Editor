using Engine.Core.Behaviours;
using Engine.Core.Entities;
using Engine.Core.Scenes.Loader;
using Engine.Core.Scenes.Loader.Info;

namespace Ginger.Editor.Ui;

public class SceneTreeViewBehaviour : IEntityBehaviour
{
    private readonly SceneLoader _sceneLoader;

    public SceneTreeViewBehaviour(SceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public void OnStart(Entity entity)
    {
        // Загрузим сцену (можно позже сделать параметром компонента)
        var scene = _sceneLoader.Load("C:\\Users\\aleks\\projects\\Engine3\\App\\resources\\scenes\\mainScene.scene.yaml");

        // Преобразуем в список TreeNode
        var nodes = new List<TreeNode>();
        foreach (var sceneChild in scene.Children)
        {
            AddEntityRecursive(sceneChild, null, nodes);
        }
        
        // Установим компонент TreeViewComponent
        entity.Children[0].ApplyComponent(new TreeViewComponent
        {
            Nodes = nodes.ToArray()
        });
    }

    private void AddEntityRecursive(EntityInfo info, string? parentId, List<TreeNode> nodes)
    {
        var id = info.Id;

        nodes.Add(new TreeNode
        {
            Id = id.ToString(),
            ParentId = parentId,
            Label = info.Name ?? "(Unnamed)",
            IsExpanded = true
        });

        foreach (var child in info.Children)
        {
            AddEntityRecursive(child, id.ToString(), nodes);
        }
    }
}