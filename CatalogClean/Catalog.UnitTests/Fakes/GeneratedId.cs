namespace Catalog.UnitTests.Fakes;

// Simula la columna IDENTITY de la base: las entidades no exponen un setter público para Id.
internal static class GeneratedId
{
    public static void Assign<TEntity>(TEntity entity, int id)
        where TEntity : class
    {
        typeof(TEntity).GetProperty("Id")!.SetValue(entity, id);
    }
}
