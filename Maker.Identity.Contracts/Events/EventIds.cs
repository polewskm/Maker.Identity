namespace Maker.Identity.Contracts.Events
{
    public static class EventIds
    {
        private const int ChangeEventsStart = 1000;

        public const int ChangeAuditInsert = ChangeEventsStart + 0;
        public const int ChangeAuditUpdate = ChangeEventsStart + 1;
        public const int ChangeAuditDelete = ChangeEventsStart + 2;

        private const int AuthenticationEventsStart = 2000;

        public const int UserLoginSuccess = AuthenticationEventsStart + 0;
        public const int UserLoginFailure = AuthenticationEventsStart + 1;
        public const int UserLogoutSuccess = AuthenticationEventsStart + 2;
    }
}