using System.Text.Json.Serialization;

namespace CapaDatos
{
    // --- DTOs PARA LA API (Clases auxiliares) ---
    // Estas clases solo sirven para "atrapar" los datos de TMDB
    public record TmdbResultDto(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("overview")] string Overview,
        [property: JsonPropertyName("poster_path")] string PosterPath,
        [property: JsonPropertyName("runtime")] int Runtime
    );

    public record TmdbSearchDto([property: JsonPropertyName("results")] List<TmdbResultDto> Results);
}