using Engine.Core.Behaviours;
using Engine.Core.Entities;
using Engine.Core.Scenes;
using Engine.Rendering;
using Engine.Core;
using Engine.Core.Transform;
using Engine.Rendering.RaylibBackend.Labels;

namespace Ginger.Editor.Ui;

public class TreeViewBehaviour : IEntityBehaviour
{
    private readonly Instantiator _instantiator;
    private World _world;
    private Prefab _labelPrefab;

    public TreeViewBehaviour(Instantiator instantiator, World world)
    {
        _instantiator = instantiator;
        _world = world;
        _labelPrefab = _instantiator.Load(
            "resources/scenes/treeviewListElement.prefab.yaml",
            new LabelPrefabParams().ToDictionary()
        );
    }

    public void OnStart(Entity entity)
    {
        entity.SubscribeComponentChange<TreeViewComponent>(_ => UpdateTree(entity));
        UpdateTree(entity);
    }

    private void UpdateTree(Entity entity)
    {
        // Удалить старые дочерние сущности (только для дерева)
        foreach (var child in entity.Children.ToList())
        {
            if (child.Name.StartsWith("TreeNode_"))
                entity.Children.Remove(child);
        }

        if (!entity.TryGetComponent<TreeViewComponent>(out var tree)) return;
        if (!entity.TryGetComponent<RectangleComponent>(out var rect)) return;

        // Построить карту: ParentId -> List<TreeNode>
        var map = new Dictionary<string, List<TreeNode>>();
        foreach (var node in tree.Nodes)
        {
            var parent = node.ParentId ?? "";
            if (!map.ContainsKey(parent))
                map[parent] = new List<TreeNode>();
            map[parent].Add(node);
        }

        // Рекурсивно создать дочерние сущности для отображения дерева
        float y = 0;
        CreateTreeEntities(entity, map, "", 0, ref y, rect.SizeCache.X);
    }

    private void CreateTreeEntities(Entity parent, Dictionary<string, List<TreeNode>> map, string parentId, int depth,
        ref float y, float width)
    {
        if (!map.TryGetValue(parentId, out var nodes)) return;
        
        var transformOverride = new TransformComponent
        {
            Transform = new Transform
            {
                Position = new System.Numerics.Vector2(10 + depth * 20, y),
                Rotation = 0,
                Scale = System.Numerics.Vector2.One
            }
        };
        
        var rectOverride = new RectangleComponent
        {
            Size = new SizeExpression { X = width.ToString(), Y = "24" }
        };
        
        foreach (var node in nodes)
        {
            var elementEntity = _instantiator.Instantiate(
                _labelPrefab,
                parent,
                [transformOverride, rectOverride]
            );
            elementEntity.Children[0].ModifySilently((ref LabelComponent label) =>
            {
                label.Text = node.Label;
                label.FontSize = 18;
            });
            elementEntity.Name = $"TreeNode_{node.Id}";

            y += 24;
            if (node.IsExpanded)
                CreateTreeEntities(parent, map, node.Id, depth + 1, ref y, width);
        }
    }
}