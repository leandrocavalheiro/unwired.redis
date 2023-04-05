namespace Unwired.Redis.Extensions;

public static class UIntExtensions
{
    public static TimeSpan ToTimeSpan(this uint expiresIn)
        => TimeSpan.FromMinutes((uint)expiresIn);

    public static TimeSpan? ToTimeSpan(this uint? expiresIn)
    {
        if (expiresIn is null || expiresIn == -1)
            return null;

        return ToTimeSpan((uint)expiresIn);
    }
}
