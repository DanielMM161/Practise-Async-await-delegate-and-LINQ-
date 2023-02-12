namespace CountryAssignment.Model;


public class Country
{
  public ComposedName? Name { get; set; }
  public string[]? Capital {get; set; }
  public string? Region { get; set; }
  public string? SubRegion { get; set; }
  public string? Flag { get; set; }
  public double? Population { get; set; }
  public string[]? Continents { get; set; }
  public string[]? Borders { get; set; }
  public Dictionary<string, string> Languages { get; set; }
  public string[]? TimeZones { get; set; }
  public string[]? Tld { get; set; }
  public CarCompose? Car { get; set; }
  public CapitalInfoCompose? CapitalInfo { get; set; }
  
  public class ComposedName
  {
    public string? Common { get; set; }
    public string? Official { get; set; }
  }

  public class CountryLanguage 
  {
    public string? Isl { get; set; }
  }

  public class CarCompose
  {
    public string? Side { get; set; }
  }

  public class CapitalInfoCompose
  {
    public double[]? Latlng { get; set; }
  }
}
