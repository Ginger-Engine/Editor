using Engine.Core;
using GignerEngine.DiContainer;
using Ginger.Editor.Ui;

namespace Ginger.Editor;

public class EditorBundle : IBundle
{
    public void InstallBindings(DiBuilder builder)
    {
        builder.Bind<TreeViewBehaviour>();
    }
    public void Configure(string c, IReadonlyDiContainer diContainer) { }
}