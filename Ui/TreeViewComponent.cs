using Engine.Core;
using System.Collections.Generic;

namespace Ginger.Editor.Ui;

public struct TreeViewComponent : IComponent
{
    public TreeNode[] Nodes;
}

public struct TreeNode
{
    public string Id;
    public string Label;
    public string ParentId; // пустая строка или null для корня
    public bool IsExpanded;
}
