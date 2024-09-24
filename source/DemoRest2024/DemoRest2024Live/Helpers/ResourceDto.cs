namespace DemoRest2024Live.Helpers;

public record ResourceDto<T>(T resource, IReadOnlyCollection<LinkDto> Links);