using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class PartySelection
{
    public int SlotIndex { get; set; }
    public PlayerType UnitType { get; set; }
    public ClassInfo ClassInfo { get; set; }
    public string Name { get; set; }

    public PartySelection(int slotIndex, PlayerType unitType, ClassInfo classInfo, string name)
    {
        SlotIndex = slotIndex;
        UnitType = unitType;
        ClassInfo = classInfo;
        Name = name;
    }
}