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
    public int currentGunIndex { get; private set; } = 0;
    public override async void _Ready()
    {
        teamSelectionHUD.OnPartyMemberAdded += AddPartyMember;
        teamSelectionHUD.OnPartyMemberRemoved += RemovePartyMember;
        teamSelectionHUD.OnCharacterChanged += ChangeModel;
        teamSelectionHUD.OnGunChanged += ChangeGun;
        teamSelectionAnimatorController.onTurnAnimationComplete += RevealWeapon;

        UpdateModelVisibility();
        UpdateWeaponVisibility();

        teamSelectionHUD.LoadClassDetails(classInfoList[currentModelIndex]);
        teamSelectionHUD.LoadClassStatDetails(classInfoList[currentModelIndex].UnitType);
        teamSelectionHUD.UpdateWeaponStats(currentGunIndex);

        await teamSelectionAnimatorController.PlayTurnAnimation(classModels[currentModelIndex]);
    }

    public override void _ExitTree()
    {
        teamSelectionHUD.OnPartyMemberAdded -= AddPartyMember;
        teamSelectionHUD.OnPartyMemberRemoved -= RemovePartyMember;
        teamSelectionHUD.OnCharacterChanged -= ChangeModel;
        teamSelectionHUD.OnGunChanged -= ChangeGun;
        teamSelectionAnimatorController.onTurnAnimationComplete -= RevealWeapon;
        base._ExitTree();
    }
    
    private async void ChangeModel(int direction)
    {
        currentModelIndex = (currentModelIndex + direction + classModels.Count) % classModels.Count;
        UpdateModelVisibility();
        UpdateWeaponVisibility();
        teamSelectionHUD.LoadClassDetails(classInfoList[currentModelIndex]);
        teamSelectionHUD.LoadClassStatDetails(classInfoList[currentModelIndex].UnitType);
        
        await teamSelectionAnimatorController.PlayTurnAnimation(classModels[currentModelIndex]);

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
            Name,
            (GunType)currentGunIndex
        );
    }

    public void RemovePartyMember(int slotIndex)
    {
        TeamSelectionManager.Instance.RemovePartyMember(slotIndex);
    }

    private void ChangeGun(int direction)
    {
        currentGunIndex = (currentGunIndex + direction + GunManager.Instance.gunModels.Count) % GunManager.Instance.gunModels.Count;
        classModels[currentModelIndex].GetNode<TeamSelectCharacter>(".").gun.SetGun((GunType)currentGunIndex);
        teamSelectionHUD.UpdateGunIcon(GunManager.Instance.gunData[currentGunIndex].Icon);
        teamSelectionHUD.UpdateWeaponStats(currentGunIndex);
    }
}
