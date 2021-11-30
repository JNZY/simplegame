namespace Game.Combat
{
    public class Damage
    {
        private Damage(float amount, DamageType type, float typeModifier)
        {
            Amount = amount;
            Type = type;
            TypeModifier = typeModifier;
        }

        public static Damage Create(float amount, DamageType type, float typeModifier)
        {
            return new Damage(amount, type, typeModifier);
        }

        public static Damage Create(float amount, DamageType type)
        {
            return new Damage(amount, type, (int) type);
        }

        public float Amount;
        public DamageType Type;
        public float TypeModifier;
    }
}