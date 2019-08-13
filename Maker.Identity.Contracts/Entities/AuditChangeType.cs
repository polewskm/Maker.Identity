namespace Maker.Identity.Contracts.Entities
{
    public enum AuditChangeType : byte
    {
        Unknown = 0,
        Insert = 1,
        Update = 2,
        Delete = 3,
    }
}