using Catalog.Application.Results;

namespace Catalog.Application.Categories;

public static class CategoryErrors
{
    public static Error NotFound(int id) => Error.NotFound($"Categoría con id '{id}' no existe.");

    public static Error NameAlreadyExists(string name) => Error.Conflict($"Ya existe una categoría con el nombre '{name}'.");

    public static Error HasProducts(string name) =>
        Error.Conflict($"No se puede eliminar la categoría '{name}' porque tiene productos asociados.");
}
