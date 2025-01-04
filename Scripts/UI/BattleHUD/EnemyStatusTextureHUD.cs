using Godot;
public partial class EnemyStatusTextureHUD :Control
{
    [Export] public Button enemyButton;
    private Character enemyCharacter;
    private float cameraY;
    private ShaderMaterial iconMaterial;
    public override void _Ready()
    {
        cameraY = CameraManager.Instance.TacticalCameraPostion.Position.Y;
        var originalMaterial = enemyButton.Material as ShaderMaterial;
        if (originalMaterial != null)
        {
            iconMaterial = originalMaterial.Duplicate() as ShaderMaterial;
            enemyButton.Material = iconMaterial;
        }
        //iconMaterial.SetShaderParameter("replace_color", new Color(1f, 0.65f, 0f, 1f));
    }

    public override void _ExitTree()
    {
        if (enemyCharacter != null)
        {
            enemyCharacter.HealthChanged -= UpdateIconColor;
        }
        base._ExitTree();
    }

    public void InitializeEnemyTexture(Character enemy)
    {
        enemyCharacter = enemy;
        enemyCharacter.HealthChanged += UpdateIconColor;
        UpdateIconColor(enemyCharacter.Health, enemyCharacter.MaxHealth); 
    }

    public void OnEnemyTextureCall()
    {
        CameraManager.Instance.TacticalCameraPostion.Position = new Vector3(enemyCharacter.Position.X, cameraY, enemyCharacter.Position.Z + cameraY/Mathf.Sqrt(2));
    }

    private void UpdateIconColor(int currentHealth, int maxHealth)
    {
        if (iconMaterial == null) return;
        
        float healthRatio = (float)currentHealth / maxHealth;
        Color newColor = new Color(1f, 1 -healthRatio, 0f, 1f);
        
        iconMaterial.SetShaderParameter("replace_color", newColor);

        if (enemyCharacter.Health <= 0)
        {
            enemyCharacter.HealthChanged -= UpdateIconColor;
            QueueFree();
        }
    }
}
