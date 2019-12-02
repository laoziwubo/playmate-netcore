namespace PlayMate.Common.Cache
{
    public interface IMemoryCache
    {
        object Get(string cacheKey);

        void Set(string cacheKey, object cacheValue);
    }
}
