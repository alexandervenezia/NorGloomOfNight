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
    private bool _beatDog;
    public bool FLAG_BEAT_DOG
    {
        get => _beatDog;
        set {
            _beatDog = value;
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

    private QuestManager() 
    {
        FLAG_TALKED_TO_MANAGER = false;
        FLAG_ACQUIRED_CROWN = false;
        FLAG_BEAT_DOG = false;
        FLAG_DELIVERED_CROWN = false;
        GD.Print("Reset quest flags");
    }

    private void SignalFlagChange()
    {
        FlagChanged?.Invoke();
    }

    public void Reset()
    {
        _flagTalkedToManager = false;
        _flagAcquiredCrown = false;
        _flagDeliveredCrown = false;
        _beatDog = false;
        GD.Print("Reset quest flags");
    }

}