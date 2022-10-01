namespace DemoRestSimonas.Data;

public class LinkDto
{
    // Url
    public string Href { get; set; }
    
    // What it does
    public string Rel { get; set; }
    
    // GET/PUT/POST
    public string Method { get; set; }
}