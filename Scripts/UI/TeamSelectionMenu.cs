using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;


public partial class TeamSelectionMenu : Control
{
    private Random rng = new Random();
    public event Action<int> OnCharacterChanged;
    public event Action OnPartyMemberAdded;
    public event Action<int> OnPartyMemberRemoved;

    #region CharacterUIInfos
    [Export] public Label characterDescriptionLabel;
    [Export] public Label characterClassLabel;
    [Export] public Label characterClassNameLabel;
    [Export] public TextureRect characterClassIcon;
    [Export] public TextureRect characterClassIcon2;
    [Export] public TextureRect characterAbilityIcon;
    [Export] public TextureRect characterAbilityIcon2;
    private ClassInfo currentClassInfo;
    #endregion

    #region Party Selection cards
    [Export] public VBoxContainer partyContainer;
    [Export] public PackedScene partySelectionCard;
    [Export] public PackedScene partySelectionEmptyCard;
    private int partyMemberCount = 0;
    private Node partySelectionEmptyCardInstance;
    #endregion

    #region Name Generation
    private static readonly string[] FirstNames = new string[]
    {
        "James", "John", "Robert", "Michael", "William",
        "David", "Richard", "Joseph", "Thomas", "Charles",
        "Daniel", "Paul", "Mark", "Donald", "George",
        "Kenneth", "Steven", "Edward", "Brian", "Ronald"
    };

    private static readonly string[] LastNames = new string[]
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones",
        "Miller", "Davis", "Anderson", "Wilson", "Taylor",
        "Moore", "Jackson", "Martin", "Thompson", "White",
        "Harris", "Clark", "Lewis", "Walker", "Hall"
    };
    #endregion

    #region Stat Selection
    private readonly string[] displayedStats = new[]
    {
        "Health",
        "Armor", 
        "Accuracy",
        "Evasion",
        "CriticalHitChance",
        "Morale"
    };

    [Export] public VBoxContainer statContainer;
    [Export] public PackedScene statBoxScene;
    private StatContainer statContainerInstance;
    #endregion

    public override void _Ready()
    {
        partySelectionEmptyCardInstance = partySelectionEmptyCard.Instantiate();
        partyContainer.AddChild(partySelectionEmptyCardInstance);
        if (TeamSelectionManager.Instance.partyMembers.Any())
            RepopulatePartyUI();
    }

    public void OnSelectMissionPressed()
    {
        if (TeamSelectionManager.Instance.partyMembers.Count < 3) return;
        GameManager.ChangeGameState(GameState.TeamSelect, GameState.MissionSelect);
    }

    public void OnCancelPressed()
    {
        GameManager.ChangeGameState(GameState.TeamSelect, GameState.Menu);
    }

    private void OnLeftArrowPressed()
    {
        OnCharacterChanged?.Invoke(-1);
    }
    
    private void OnRightArrowPressed()
    {
        OnCharacterChanged?.Invoke(1);
    }

    public void LoadClassDetails(Resource classInfo)
    {
        if (classInfo == null) return;

        currentClassInfo = (ClassInfo)classInfo;

        characterDescriptionLabel.Text = currentClassInfo.DescriptionText;
        characterClassLabel.Text = currentClassInfo.DetailsText;
        characterClassNameLabel.Text = currentClassInfo.DetailsText;
        characterClassIcon.Texture = currentClassInfo.ClassIcon;
        characterClassIcon2.Texture = currentClassInfo.ClassIcon;
        characterAbilityIcon.Texture = currentClassInfo.AbilityIcon;
        characterAbilityIcon2.Texture = currentClassInfo.AbilityIcon2;
    }

    public void LoadClassStatDetails(PlayerType playerType)
    {
        foreach (Node child in statContainer.GetChildren())
            child.QueueFree();
        
        var statResource = PlayerStatsFactory.CreateStatsForPlayerType(playerType);
        var stats = new PlayerStats(playerType, statResource);

        foreach (var stat in displayedStats)
        {
            var statBox = statBoxScene.Instantiate<CharacterStatBoxCard>();
            int statValue = stats.GetStatByName(stat).GetValue();
            statBox.LoadStatDetails(stat, statValue);
            statContainer.AddChild(statBox);
        }
    }

    public void AddPartyMember()
    {
        if (currentClassInfo == null || partyMemberCount >= 3) return;
        
        var instance = partySelectionCard.Instantiate<CharacterUIUnitCard>();
        instance.InitCard(this, currentClassInfo, partyContainer.GetChildCount() - 1, GenerateCharacterName());
        partyContainer.AddChild(instance);
        partyMemberCount++;
        CheckPartyMemberCount();
        OnPartyMemberAdded?.Invoke();
    }

    
    public void RemovePartyMember(CharacterUIUnitCard card, int slotIndex)
    {
        partyContainer.RemoveChild(card);
        partyMemberCount--;
        CheckPartyMemberCount();
        OnPartyMemberRemoved?.Invoke(slotIndex);
    }

    /*
    public void ResetPartyUI()
    {
        foreach (var child in partyContainer.GetChildren())
            child.QueueFree();
        partyMemberCount = 0;
        CheckPartyMemberCount();
    }

    */
    public void RepopulatePartyUI()
    {
        partyMemberCount = 0;
        foreach(var member in TeamSelectionManager.Instance.partyMembers)
        {
            var instance = partySelectionCard.Instantiate<CharacterUIUnitCard>();
            instance.InitCard(this, member.ClassInfo, partyContainer.GetChildCount() - 1, member.Name);
            partyContainer.AddChild(instance);
            partyMemberCount++;
        }
        CheckPartyMemberCount();
    }


    public string GenerateCharacterName()
    {
        int firstNameIndex = rng.Next(0, FirstNames.Length);
        int lastNameIndex = rng.Next(0, LastNames.Length);
        return $"{FirstNames[firstNameIndex]} {LastNames[lastNameIndex]}";
    }

    private void CheckPartyMemberCount()
    {
        var currentParent = partySelectionEmptyCardInstance.GetParent();
        
        if (partyMemberCount >= 3)
        {
            if (currentParent == partyContainer)
                partyContainer.RemoveChild(partySelectionEmptyCardInstance);
        }
        else
        {
            if (currentParent != partyContainer)
            {
                partyContainer.AddChild(partySelectionEmptyCardInstance);
                partyContainer.MoveChild(partySelectionEmptyCardInstance, 0);
            }
            else
            {
                partyContainer.MoveChild(partySelectionEmptyCardInstance, 0);
            }
        }
    }
    
}
