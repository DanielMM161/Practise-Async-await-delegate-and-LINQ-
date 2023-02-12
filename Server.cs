using System.Text.Json;
using CountryAssignment.Model;
using CountryAssignment.Interfaces;
using static System.Console;

namespace CountryAssignment
{
  public class Server : ICountryInfo
  {
    private static Server _instance;
    private HttpClient _httpClient { get; }

    private Server() 
    {
      _httpClient = new HttpClient();
    }

    public static Server GetInstance()
    {
      if(_instance == null)
      {
        _instance = new Server();
      }
      return _instance;
    }

    private async Task<string> GetResponse(string endPoint)
    {
      var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/{endPoint}");
      return await response.Content.ReadAsStringAsync();
    }

    private T? DeserializeCountries<T>(string body)
    {
      T? result = default(T);
      try
      {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        result = JsonSerializer.Deserialize<T>(body, options);  

      }catch(Exception ex)
      {
        WriteLine($"Exception Deserializing {ex.Message}");
      }
      return result;
    }

    private async Task<Country> GetCountryByName(string name)
    {      
      var body = await GetResponse($"name/{name}");
      var result = DeserializeCountries<List<Country>>(body);
      return result[0] ?? new Country();
    }

    private async Task<List<Country>> GetAllCountries()
    {      
      var body = await GetResponse($"all");
      var result = DeserializeCountries<List<Country>>(body);
      return result ?? new List<Country>();
    }

    public async Task<ICollection<Country>> GetAllAsync()
    {      
      return await GetAllCountries();
    }

    public async Task<ICollection<string>> GetAllSpokenLanguagesAsync()
    {
      List<Country> countries = await GetAllCountries();
      var languagesSets = new HashSet<string>();
      
      countries.ForEach(country => {
        if(country.Languages != null)
        {
          country.Languages.Values
            .ToList()
            .ForEach(item => {
              languagesSets.Add(item);
            });                    
        }        
      });

      return languagesSets;
    }

    public async Task<ICollection<Country>> GetBorderedWithAsync(string countryCode)
    {
      var countries = await GetAllCountries();
      Func<Country, bool> predicate = (country) => {
        if(country.Borders != null && country.Borders.Count() > 0)
        {
          return country.Borders.Any(border => border.ToUpper().Equals(countryCode.ToUpper()));
        }
        return false;
      };

      return countries
        .Where(predicate)
        .ToList();      
    }

    public async Task<ICollection<Country>> GetByCommonNameAsync(string commonName)
    {      
      var body = await GetResponse($"translation/{commonName}");
      var result = DeserializeCountries<List<Country>>(body);
      return result ?? new List<Country>();     
    }

    public async Task<ICollection<Country>> GetByCurrencyAsync(string currencyCode)
    {
      var body = await GetResponse($"currency/{currencyCode}");
      var result = DeserializeCountries<List<Country>>(body);
      return result ?? new List<Country>();  
    }

    public async Task<ICollection<Country>> GetByLanguageAsync(string langCode)
    {
      var body = await GetResponse($"lang/{langCode}");
      var result = DeserializeCountries<List<Country>>(body);
      return result ?? new List<Country>();  
    }

    public async Task<ICollection<Country>> GetCaptialOnNorthHemisphereAsync()
    {
      var countries = await GetAllCountries();      
      return countries.Where(country => country.CapitalInfo?.Latlng?[0] >= 1).ToList();
    }

    public async Task<ICollection<string>> GetCloseTimesAsync()
    {
      var country = await GetCountryByName("Finland");
      var countries = await GetAllCountries();

      Func<string, int> getDigit = (str) => {              
        int result = 0;
        var strDigit = "";
        foreach(var caracter in str)
        {
          if(caracter.Equals(':'))
          {
            result = Int32.Parse(strDigit);            
            break;
          }

          if(Char.IsDigit(caracter))
          { 
            strDigit += caracter;                      
          }
        }

        return result;
      };

      int reference = getDigit(country.TimeZones?[0] ?? "");      

      Func<Country, bool> predicate = (country) => {
        foreach(var time in country.TimeZones)
        {
          int digit = 0;
          if(time.Contains('+'))
          {
            digit = getDigit(time);
            if( digit >= reference && digit <= reference + 5)
            {
              return true;
            }
          } else if(time.Contains('-'))
          {
            digit = getDigit(time);
            if( digit <= reference && digit >= reference - 5)
            {
              return true;
            }
          } else
          {
            digit = digit - reference;
            if( digit >= reference && digit <= reference + 5)
            {
              return true;
            }
          }
        }
        return false;
      };

      return countries
        .Where(predicate)
        .Select(item => item.Name?.Common ?? "")
        .ToList();
    }

    public async Task<Country> GetCountryByDomainAsync(string url)
    {      
      char[] charArr = url.ToCharArray();
      int dotIndex = 0;
      for(int i = charArr.Count() - 1; i >= 0; i--)
      {        
        if(charArr[i].Equals('.'))
        {
          dotIndex = i;
          break;
        }
      }

      string domain = url.Substring(dotIndex);

      Predicate<Country> predicate = (country) => {
        var tld = country.Tld;
        if(tld != null)
        {          
          return tld
            .ToList()
            .Any(item => {
              if(item != null)
              {
                return item.Equals(domain);
              }
              return false;
            });        
        }
        return false;
      };

      var countries = await GetAllCountries();
      Country? result = countries.Find(predicate);

      return result ?? new Country();
    }    

    public async Task<ICollection<Country>> GetDriveOnLeftAsync()
    {
      var countries = await GetAllCountries();
      var countriesDriveLeft = countries.Where(country => {
        if(country.Car != null && country.Car.Side.ToLower().Equals("left"))
        {
          return true;
        }
        return false;
      });

      return countriesDriveLeft.ToList();
    }

    public async Task<ICollection<string>> GetSpokenLanguagesAsync(string countryCode)
    {
      var body = await GetResponse($"lang/{countryCode}");
      var result = DeserializeCountries<List<Country>>(body);      
      return result.Select(item => item.Name?.Common ?? "").ToList() ?? new List<string>();
    }

    public async Task<ICollection<Country>> GetTopPopulatedAsync(int top)
    {
      var countries = await GetAllCountries();
      countries.Sort((a, b) => {
        if(a.Population < b.Population)
        {
          return 1;
        } else if(a.Population == b.Population) {
          return 0;
        }
        return -1;
      });

      return countries.Take(top).ToList();
    }

    public async Task PrintPeopleInfoAsync(List<(string name, string countryName)> peoples)
    {
      foreach(var people in peoples)
      {
        var country = await GetCountryByName(people.countryName);        
        var languages = String.Join(" or ", country.Languages.Values);
        WriteLine($"{people.name} probably speaks {languages}");        
      }
    }
  }
}
