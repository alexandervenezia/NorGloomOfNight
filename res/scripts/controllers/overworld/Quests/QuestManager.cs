/*
 The quest system in general is really stupidly implemented due to time pressure. Apologies.
*/

using Godot;

namespace Overworld;

public class QuestManager
{
    private bool _flagTalkedToManager;
    public bool FLAG_TALKED_TO_MANAGER
    {
        get => _flagTalkedToManager;
        set {
            _flagTalkedToManager = value;
            SignalFlagChange();
        }
    }
    private bool _flagAcquiredCrown;
    public bool FLAG_ACQUIRED_CROWN
    {
        get => _flagAcquiredCrown;
        set {
            _flagAcquiredCrown = value;
            SignalFlagChange();
        }
    }
    private bool _flagDeliveredCrown;
    public bool FLAG_DELIVERED_CROWN
    {
        get => _flagDeliveredCrown;
        set {
            _flagDeliveredCrown = value;
            SignalFlagChange();
        }
    }

    public delegate void FlagChangeInformer();
    public event FlagChangeInformer FlagChanged;

    private static QuestManager _instance;
    public static QuestManager GetInstance()
    {
        if (_instance == null)
            _instance = new QuestManager();
        return _instance;
    }

    private QuestManager() {}

    private void SignalFlagChange()
    {
        GD.Print("Test");
        GD.Print(FLAG_TALKED_TO_MANAGER);
        FlagChanged?.Invoke();
    }
}