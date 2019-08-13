namespace Maker.Identity.Contracts.Audit
{
    public enum ChangeType : byte
    {
        Unknown = 0,
        Insert = 1,
        Update = 2,
        Delete = 3,
    }
}