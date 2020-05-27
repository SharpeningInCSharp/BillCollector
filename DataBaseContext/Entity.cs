namespace DataBaseContext
{
	public abstract class Entity
	{
		internal Entity(EntityType entityType)
		{
			EntityType = entityType;
		}

		internal EntityType EntityType { get; }
	}
}
