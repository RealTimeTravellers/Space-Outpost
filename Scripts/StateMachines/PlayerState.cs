using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class PlayerState : BaseState<PlayerStateType>
{
    public override PlayerStateType Process(Character character)
    {
        return PlayerStateType.Idle;
    }
}