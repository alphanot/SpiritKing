using Microsoft.Xna.Framework;
using SpiritKing.Data;
using static SpiritKing.Data.SettingsData;

namespace SpiritKing.Components.Nodes;
public class SaveSlotMenuButton : MenuButton
{
    public bool IsOccupied { get; set; } = false;

    public float Progress { get; set; }

    public string FileName { get; set; }

    public override string Text
    {
        get
        {
            return IsOccupied ? _text : "Empty";
        }

        set
        {
            _text = value;
        }
    }

    private string _text = string.Empty;

    public SaveSlotMenuButton(Game game, Point position, Point size, SaveSlot saveSlot)
        : base(game, position, size, string.Empty)
    {
        IsOccupied = saveSlot.IsOccupied;
        Text = saveSlot.Name;
        Progress = saveSlot.Progress;
        FileName = saveSlot.FileName;
    }
}
