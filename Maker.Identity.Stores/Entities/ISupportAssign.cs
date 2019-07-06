namespace Maker.Identity.Stores.Entities
{
	public interface ISupportAssign<in T>
		where T : ISupportAssign<T>
	{
		void Assign(T other);
	}
}