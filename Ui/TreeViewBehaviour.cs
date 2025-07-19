using Engine.Core.Behaviours;
using Engine.Core.Entities;
using Engine.Core.Scenes;
using Engine.Rendering;
using Engine.Core;
using Engine.Core.Prefabs;
using Engine.Core.Transform;
using Engine.Rendering.RaylibBackend.Labels;

namespace Ginger.Editor.Ui;

public class TreeViewBehaviour : IEntityBehaviour
{
    private readonly PrefabInstantiator _prefabInstantiator;
    private readonly Prefab _labelPrefab;
    private readonly SceneManager _sceneManager;
    private readonly EntityBehaviourManager _entityBehaviourManager;

    public TreeViewBehaviour(PrefabInstantiator prefabInstantiator, SceneManager sceneManager,
        EntityBehaviourManager entityBehaviourManager)
    {
        _prefabInstantiator = prefabInstantiator;
        _sceneManager = sceneManager;
        _entityBehaviourManager = entityBehaviourManager;
        _labelPrefab = _prefabInstantiator.Load(
            "resources/scenes/label.prefab.yaml",
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
            if (child.Name != null && child.Name.StartsWith("TreeNode_"))
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
        foreach (var node in nodes)
        {
            var labelOverride = _labelPrefab.RootEntity.GetComponent<LabelComponent>();
            labelOverride.Text = node.Label;
            labelOverride.FontSize = 18;

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
            var labelEntity = _prefabInstantiator.Instantiate(
                _labelPrefab,
                new IComponent[] { labelOverride, transformOverride, rectOverride }
            );
            labelEntity.Name = $"TreeNode_{node.Id}";
            parent.Children.Add(labelEntity);
            labelEntity.Parent = parent;
            _sceneManager.AddEntityToCurrentScene(labelEntity);

            y += 24;
            if (node.IsExpanded)
                CreateTreeEntities(parent, map, node.Id, depth + 1, ref y, width);
        }
    }
}