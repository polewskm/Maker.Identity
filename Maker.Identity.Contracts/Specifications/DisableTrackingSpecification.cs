namespace Maker.Identity.Contracts.Specifications
{
    public class DisableTrackingSpecification<TEntity> : IQuerySpecification<TEntity>
    {
        public string Name => QueryNames.DisableTracking;

        public bool DisableTracking { get; }

        public DisableTrackingSpecification(bool disableTracking)
        {
            DisableTracking = disableTracking;
        }

    }
}