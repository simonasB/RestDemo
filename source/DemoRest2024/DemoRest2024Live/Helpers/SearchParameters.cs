namespace DemoRest2024Live.Helpers;

public class SearchParameters
{
    private int _pageNumber = 1;
    private int _pageSize = 2;

    private const int MaxPageSize = 50;
    
    public int? PageNumber
    {
        get => _pageNumber; 
        set => _pageNumber = value is null or <= 0 ? _pageNumber : value.Value;
    }

    public int? PageSize
    {
        get => _pageSize > MaxPageSize ? MaxPageSize : _pageSize;
        set => _pageSize = value is null or <= 0 ? _pageSize : value.Value;
    }
}