namespace SimpleECS
{
    public readonly struct Entity
    {
        public readonly uint Id;

        public Entity(uint id) => Id = id;

        public static implicit operator Entity(uint id) => new Entity(id);

        public override int GetHashCode() => Id.GetHashCode();
    }
}