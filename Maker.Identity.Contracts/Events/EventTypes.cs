namespace Maker.Identity.Contracts.Events
{
    public enum EventTypes
    {
        Unknown = 0,
        Error = 1,
        Warning = 2,
        Information = 4,
        SuccessAudit = 8,
        FailureAudit = 16,
        ChangeAudit = 32,
    }
}