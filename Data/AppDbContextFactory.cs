using BuenosAiresExp.Data;

namespace BuenosAiresExp.Services
{
    internal static class AppDbContextFactory
    {
        private static readonly Lazy<AppDbContext> _sharedContext =
            new Lazy<AppDbContext>(() => new AppDbContext());

        public static AppDbContext GetSharedContext()
        {
            return _sharedContext.Value;
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }
    }
}
