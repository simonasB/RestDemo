namespace DemoRest20232.Helpers;

public record ResourceDto<T>(T Resource, IReadOnlyCollection<LinkDto> Links);