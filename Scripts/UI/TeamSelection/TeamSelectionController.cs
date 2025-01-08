using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;


public partial class TeamSelectionController : Node
{
    [Export] public Sprite3D background;
    [Export] public Godot.Collections.Array<Node3D> classModels;
    [Export] public Godot.Collections.Array<ClassInfo> classInfoList;
    [Export] public Godot.Collections.Array<StatContainer> statContainers;
    [Export] public Godot.Collections.Array<Node3D> classGunModels;
    public List<PartySelection> partyMembers = new();
    [Export] public TeamSelectionMenu teamSelectionHUD;
    [Export] public TeamSelectionAnimatorController teamSelectionAnimatorController;
    public int currentModelIndex { get; private set; } = 0;
    public override void _Ready()
    {
        teamSelectionHUD.OnPartyMemberAdded += AddPartyMember;
        teamSelectionHUD.OnPartyMemberRemoved += RemovePartyMember;
        teamSelectionHUD.OnCharacterChanged += ChangeModel;
        teamSelectionAnimatorController.onTurnAnimationComplete += RevealWeapon;

        UpdateModelVisibility();
        UpdateWeaponVisibility();
        teamSelectionAnimatorController.PlayTurnAnimation(classModels[currentModelIndex]);
        teamSelectionHUD.LoadClassDetails(classInfoList[currentModelIndex]);
        teamSelectionHUD.LoadClassStatDetails(classInfoList[currentModelIndex].UnitType);
    }

    public override void _ExitTree()
    {
        teamSelectionHUD.OnPartyMemberAdded -= AddPartyMember;
        teamSelectionHUD.OnPartyMemberRemoved -= RemovePartyMember;
        teamSelectionHUD.OnCharacterChanged -= ChangeModel;
        teamSelectionAnimatorController.onTurnAnimationComplete -= RevealWeapon;
        base._ExitTree();
    }
    
    private void ChangeModel(int direction)
    {
        currentModelIndex = (currentModelIndex + direction + classModels.Count) % classModels.Count;
        UpdateModelVisibility();
        UpdateWeaponVisibility();
        teamSelectionAnimatorController.PlayTurnAnimation(classModels[currentModelIndex]);
        teamSelectionHUD.LoadClassDetails(classInfoList[currentModelIndex]);
        teamSelectionHUD.LoadClassStatDetails(classInfoList[currentModelIndex].UnitType);
    }
    
    private void UpdateModelVisibility()
    {
        for (int i = 0; i < classModels.Count; i++)
        {
            classModels[i].Visible = i == currentModelIndex;
        }
    }

    private void RevealWeapon()
    {
        classGunModels[currentModelIndex].Visible = true;
    }

    private void UpdateWeaponVisibility(){
        for (int i = 0; i < classModels.Count; i++)
        {
            classGunModels[i].Visible = i == currentModelIndex && classInfoList[i].WeaponDisplayType == WeaponDisplayType.WithWeapon;
        }
    }

    public void AddPartyMember(string Name)
    {
        TeamSelectionManager.Instance.AddPartyMember(
            classInfoList[currentModelIndex].UnitType,
            TeamSelectionManager.Instance.GetPartyMembers().Count,
            classInfoList[currentModelIndex],
            Name
        );
    }

    public void RemovePartyMember(int slotIndex)
    {
        TeamSelectionManager.Instance.RemovePartyMember(slotIndex);
    }
}
