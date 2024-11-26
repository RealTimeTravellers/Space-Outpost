using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class CharacterState : BaseState<CharacterStateType>
{
    public override CharacterStateType Process(Character character)
    {
        return CharacterStateType.Idle;
    }
}