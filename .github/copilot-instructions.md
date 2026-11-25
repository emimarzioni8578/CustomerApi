# Copilot Instructions

## Project Guidelines
- En esta solución, los servicios/handlers deben informar los fallos esperados (no encontrado, conflicto, validación) con el patrón Result (Result / Result<T> + Error con ErrorType) y los controllers deben revisar result.IsFailure y devolver result.Error.ToProblem(), como en ProductCatalogApi; no usar excepciones para esos casos.