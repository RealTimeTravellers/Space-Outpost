using Godot;

public partial class GridObject : Node3D
{
	private bool _isOccupied = false;
    private bool _isBlocked = false;
    private Character _occupyingCharacter;
	[Export] public CoverType coverType = CoverType.None;
	public bool HasCover { get; private set; }
	public Vector3 CoverNormal { get; private set; }
	[Export] public Godot.Collections.Dictionary<string, CoverType> coverDirection = new() { {"-z" , 0}, {"x" , 0}, {"z" , 0}, {"-x" , 0} };
	
	[Export] private bool isSpriteOnly = true;

	[Export] private CollisionShape3D collisionShape;

	#region 3D Variables and Materials
	[Export] public Material standardMaterial;
	[Export] public Material selectedMaterial;
	[Export] public Material innerMaterial;
	[Export] public Material outerMaterial;
	[Export] public Material blockedMaterial;

	[Export] private float standardTransparency = 0.75f;
	[Export] private float selectedTransparency = 0.25f;
	
	[Export] private MeshInstance3D meshInstance;
	#endregion

	#region 2D Variables and Values
	[Export] Sprite3D gridSprite;

	[Export] public Color standardColour;
	[Export] public Color selectedColour;
	[Export] public Color innerColour;
	[Export] public Color outerColour;
	[Export] public Color blockedColour;

    public bool IsOccupied 
    { 
        get => _isOccupied; 
        set => _isOccupied = value; 
    }

    public bool IsBlocked 
    { 
        get => _isBlocked; 
        set => _isBlocked = value; 
    }
	public Character OccupyingCharacter
    {
        get => _occupyingCharacter;
        set
        {
            _occupyingCharacter = value;
            _isOccupied = value != null;
        }
    }
	#endregion
	public override void _Ready()
	{
		SubscribeToEvents();
		meshInstance.SetSurfaceOverrideMaterial(0, standardMaterial);
		if (isSpriteOnly)
			meshInstance.Visible = false;
		base._Ready();
	}

	private void SubscribeToEvents()
	{
		GridManager.Instance.SelectionChanged += OnSelectionChanged;
		TurnManager.Instance.PlayerMovementChanged += OnPlayerMovementChanged;
		TurnManager.Instance.TurnChanged += OnTurnChanged;
	}

	private void ChangeGridMaterial(Material mat, float transparency, Color colour, bool spriteOnly)
	{
		if (spriteOnly)
		{
			if(transparency == 1)
			{
				collisionShape.Disabled = true;
				gridSprite.Visible = false;
			}
			else
			{
				collisionShape.Disabled = false;
				gridSprite.Visible = true;
			}

			gridSprite.Modulate = colour;
			//gridSprite.Transparency = transparency;
		}
		else
		{
			meshInstance.SetSurfaceOverrideMaterial(0, mat);
			meshInstance.Transparency = transparency;
		}
	}

	private void UpdateColour(GridObject gridObject)
	{
		// TODO: if selected colour it as selected (Change Material on Geometry3D)
		if (gridObject == null)
		{   
			ChangeGridMaterial(standardMaterial, standardTransparency, standardColour, isSpriteOnly);
			return;
		}

		if (gridObject.GetInstanceId() == this.GetInstanceId()) // since I do a null check above, this is a little faster
			ChangeGridMaterial(selectedMaterial, selectedTransparency, selectedColour, isSpriteOnly);
		else
		{
			// TODO: if this is not selected change colour based on range or enemy presence
			if (GridManager.Instance.selectedCharacter != null)
			{
				var character = GridManager.Instance.selectedCharacter;
				if (character.actionPoints <= 0)
				{
					if (this.Position.DistanceTo(character.Position) < character.FirstMovementRange + character.SecondMovementRange)
						ChangeGridMaterial(blockedMaterial, standardTransparency, blockedColour, isSpriteOnly);
					else
						ChangeGridMaterial(standardMaterial, 1f, standardColour, isSpriteOnly);

					return;
				}
				
				if (this.Position.DistanceTo(character.Position) < character.FirstMovementRange)
					ChangeGridMaterial(innerMaterial, standardTransparency, innerColour, isSpriteOnly);
				else if (this.Position.DistanceTo(character.Position) < character.FirstMovementRange + character.SecondMovementRange)
					ChangeGridMaterial(outerMaterial, standardTransparency, outerColour, isSpriteOnly);
				else
					ChangeGridMaterial(standardMaterial, 1f, standardColour, isSpriteOnly);
			}
		}
	}

	private void OnPlayerMovementChanged(bool started)
	{
		if(!started)
			UpdateColour(TurnManager.CurrentlyMovingCharacter.currentGrid);
	}

	private void OnSelectionChanged(GridObject gridObject)
	{
		UpdateColour(gridObject);
	}

	private void OnTurnChanged(bool playerTurn)
	{
		if (playerTurn)
			ChangeGridMaterial(standardMaterial, standardTransparency, standardColour, isSpriteOnly);
		else
			ChangeGridMaterial(blockedMaterial, standardTransparency, blockedColour, isSpriteOnly);
	}

	private void CheckCoverStatus()
    {
        // Raycast ile etrafı kontrol et
        var space = GetWorld3D().DirectSpaceState;
        
        // 4 yönü kontrol et (sağ, sol, ön, arka)
        Vector3[] directions = {
            Vector3.Right,
            Vector3.Left,
            Vector3.Forward,
            Vector3.Back
        };

        foreach (var dir in directions)
        {
            var query = PhysicsRayQueryParameters3D.Create(
                GlobalPosition + Vector3.Up,
                GlobalPosition + Vector3.Up + dir * 2
            );
            query.CollisionMask = 1 << 1; // Layer 1 (walls)
            var result = space.IntersectRay(query);

            if (result.Count > 0)
            {
                HasCover = true;
                CoverNormal = -dir; // Cover'ın normal'i duvarın tersi yönünde
                coverType = CoverType.Full;
                break;
            }
        }
    }

	public void SetOccupied(Character character)
    {
        OccupyingCharacter = character;
    }

    public void ClearOccupied()
    {
        OccupyingCharacter = null;
    }
}
