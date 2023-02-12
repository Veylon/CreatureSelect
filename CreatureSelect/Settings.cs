using UnityModManagerNet;

namespace CreatureSelect
{
    public class Settings : UnityModManager.ModSettings
    {
        public float MyFloatOption = 2f;
        public bool MyBoolOption = true;
        public string MyTextOption = "Hello";

        public string MinCR = "1";
        public string MaxCR = "30";
        public string TextBox = "";

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}