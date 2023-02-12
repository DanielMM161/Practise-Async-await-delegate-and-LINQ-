using CountryAssignment.Model;

namespace CountryAssignment.Interfaces
{
  public interface ICountryInfo
  {
    // Get countries by common name. Note that the name can be in different languages. So given Finlandia in Italian, it should return Finland
    Task<ICollection<Country>> GetByCommonNameAsync(string commonName);

    // Get countries by language code
    Task<ICollection<Country>> GetByLanguageAsync(string langCode);

    // Get countries by currency code
    Task<ICollection<Country>> GetByCurrencyAsync(string currencyCode);

    // Get countries that speak a language of a certain code
    Task<ICollection<string>> GetSpokenLanguagesAsync(string countryCode);

    // Get list of all languages spoken in the world
    Task<ICollection<string>> GetAllSpokenLanguagesAsync();

    // Get all countries
    Task<ICollection<Country>> GetAllAsync();

    // Get top populated countries
    Task<ICollection<Country>> GetTopPopulatedAsync(int top);

    // Get countries that border with a country that has a certain code
    Task<ICollection<Country>> GetBorderedWithAsync(string countryCode);

    // Get countries that drive car on left
    Task<ICollection<Country>> GetDriveOnLeftAsync();

    // Get list of countries that has capital center on the north hemisphere
    Task<ICollection<Country>> GetCaptialOnNorthHemisphereAsync();

    // Get the country that has the domain name from the url.
    // For example, https://yle.fi would return Finland
    Task<Country> GetCountryByDomainAsync(string url);

    // Get list of country names where their current time is
    // within 5 hours (more or less) of the current Finnish time.
    Task<ICollection<string>> GetCloseTimesAsync();

    // Given a list of wellknown people (you come up with) with name and country code,
    // print out something like so with the param of
    // List<(string name, string countryCode)> params = new
    // {
    //    ("Messi", "Argentina"),
    //    ("Linus Torvald", "Finland"),
    // }
    // * Messi probably speaks Guaraní or Spanish
    // * Linus Torvald probably speaks Finnish or Swedish
    Task PrintPeopleInfoAsync(List<(string name, string countryName)> people);
  }
}