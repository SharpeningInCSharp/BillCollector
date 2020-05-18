using System.ComponentModel.DataAnnotations;

namespace DataBaseContext
{
	public abstract class IEntity
	{
		internal IEntity(EntityType entityType)
		{
			EntityType = entityType;
		}

		internal EntityType EntityType { get; }
	}
}
