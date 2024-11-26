public partial class Guardian : PrimaryWeapon
{
	public Guardian()
	{
		Name = "Guardian Light Machine Gun";
		Description = "Dengeli bir silah olup, sürekli ateşleme kapasitesi sayesinde savunma ve destek görevlerinde etkilidir.";
		PlayerType = PlayerType.Medic;
		PrimaryWeaponType = PrimaryWeaponType.Guardian;
		MaxAmmoClip = 3;
		CurrentAmmoClip = MaxAmmoClip;
		Accuracy = 12;
		Range = 12;
		MinDamage = 3;
		MaxDamage = 5;
		Impact = 4;
		CritChance = 10;
	}
}
