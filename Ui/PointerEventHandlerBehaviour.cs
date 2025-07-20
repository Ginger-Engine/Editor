using System.Drawing;
using Engine.Core.Behaviours;
using Engine.Core.Entities;
using Engine.Input.PointerEvents;
using Engine.Rendering.RaylibBackend.Labels;

namespace Ginger.Editor.Ui;

public class PointerEventHandlerBehaviour(PointerInputManager pointerInputManager) : IEntityBehaviour
{
    public void OnStart(Entity entity)
    {
        pointerInputManager.Subscribe<EnterEvent>(entity, _ =>
        {
            entity.Children[0].Modify((ref LabelComponent label) =>
            {
                label.Color = Color.Blue;
            });
        });
        pointerInputManager.Subscribe<LeaveEvent>(entity, _ =>
        {
            entity.Children[0].Modify((ref LabelComponent label) =>
            {
                label.Color = Color.Black;
            });
        });
    }

    public void OnDestroy(Entity entity)
    {
        pointerInputManager.Unsubscribe<EnterEvent>(entity.Children[0]);
        pointerInputManager.Unsubscribe<LeaveEvent>(entity.Children[0]);
    }
}