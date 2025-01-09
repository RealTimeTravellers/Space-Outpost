using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;


public partial class TeamSelectionManager : Node
{
    public static TeamSelectionManager Instance { get; private set; }
    public List<PartySelection> partyMembers {get; set;} = new();
    
    public override void _Ready()
    {
        if (Instance == null){
            Instance = this;
            GameManager.GameStateChanged += OnGameStateChanged;
        }
        else
            QueueFree();
    }

    public override void _ExitTree()
    {
        GameManager.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState current, GameState newState)
    {
        if (current == GameState.TeamSelect && newState == GameState.Menu)
            ResetParty();
    }

    public void AddPartyMember(PlayerType unitType, int slotIndex, ClassInfo classInfo, string name, GunType gunType)
    {
        partyMembers.Add(new PartySelection(slotIndex, unitType, classInfo, name, gunType));
    }

    public void RemovePartyMember(int slotIndex)
    {
        var memberToRemove = partyMembers.FirstOrDefault(p => p.SlotIndex == slotIndex);
        if (memberToRemove != null)
        {
            partyMembers.Remove(memberToRemove);
            for (int i = 0; i < partyMembers.Count; i++)
                partyMembers[i].SlotIndex = i;
        }
    }

    public void ResetParty()
    {
        if(partyMembers.Count > 0)
            partyMembers.Clear();
        
        TurnManager.Instance.isGameOver = false;
    }

    public List<PartySelection> GetPartyMembers()
    {
        return partyMembers;
    }
}