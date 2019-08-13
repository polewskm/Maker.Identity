namespace Maker.Identity.Contracts.Audit
{
    public static class EventIds
    {
        private const int AuthenticationEventsStart = 1000;

        public const int UserLoginSuccess = AuthenticationEventsStart + 0;
        public const int UserLoginFailure = AuthenticationEventsStart + 1;
        public const int UserLogoutSuccess = AuthenticationEventsStart + 2;
    }
}