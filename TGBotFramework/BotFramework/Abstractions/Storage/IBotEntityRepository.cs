namespace BotFramework.Abstractions.Storage
{
    public interface IBotEntityRepository<TEntity, in TKey> where TEntity : IBotEntity
    {
        TEntity Get(TKey key);

        TEntity Update(TEntity entity);

        void Delete(TEntity entity);

        TEntity Create(TEntity entity);
    }
}
