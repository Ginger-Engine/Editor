using System.Collections.Generic;
using System.Numerics;

namespace Ginger.Editor.Ui;

public struct LabelPrefabParams()
{
    public string Text = "";
    public int FontSize = 24;
    public string Font = "OpenSans";
    public string Color = "#FF000000";
    public object Size = new Dictionary<string, object> { ["x"] = 100, ["y"] = 24 };
    public object Position = new Dictionary<string, object> { ["x"] = 0, ["y"] = 0 };
    public float Rotation = 0f;
    public object Scale = new Dictionary<string, object> { ["x"] = 1, ["y"] = 1 };
    public string Layer = "Ui";

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            ["Text"] = Text,
            ["FontSize"] = FontSize,
            ["Font"] = Font,
            ["Color"] = Color,
            ["Size"] = Size,
            ["Position"] = Position,
            ["Rotation"] = Rotation,
            ["Scale"] = Scale,
            ["Layer"] = Layer
        };
    }
} 