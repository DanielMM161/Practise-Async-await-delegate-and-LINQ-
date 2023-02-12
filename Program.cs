using CountryAssignment;
using CountryAssignment.Model;
using static System.Console;
internal class Program
{
  
  private static void ShowMessage<T>(string message, ICollection<T> result, Action<string> action)
  {
    WriteLine($"----------- {message} -----------");    
    foreach(var item in result)
    {
      if(typeof(T).Equals(typeof(Country)))
      {
        Country country = (Country)Convert.ChangeType(item, typeof(Country));
        action(country?.Name?.Common ?? "");      
      } else
      {
        string element = item.ToString();
        action(element);         
      }
    }
    WriteLine(" ");
  }
  private static async Task Main(string[] args)
  {
    Server server = Server.GetInstance();
    Action<string> action = (string message) => WriteLine(message);    
    
    var commonNameResult = await server.GetByCommonNameAsync("Grónsko");
    ShowMessage("Country By Common Name -- Grónsko", commonNameResult, action);    
    
    var countryByCode = await server.GetByLanguageAsync("FIN");
    ShowMessage("Country By Language Code -- FIN", countryByCode, action);

    var countryCurrency = await server.GetByCurrencyAsync("dollar");
    ShowMessage("Country By Currency Code -- dollar", countryCurrency, action);

    var countrySpeak = await server.GetSpokenLanguagesAsync("ENG");
    ShowMessage("Get Country Speak Language By Language Code -- ENG", countrySpeak, action);

    var spokenLanguage = await server.GetAllSpokenLanguagesAsync();
    ShowMessage("Get All Spoken Language", spokenLanguage, action);

    var topPopulated = await server.GetTopPopulatedAsync(5);
    ShowMessage("Get Top Populated", topPopulated, action);

    var countriesBordered = await server.GetBorderedWithAsync("ESP");
    ShowMessage("Get Bordered Country with -- ESP", countriesBordered, action);

    var countriesDriveLeft = await server.GetDriveOnLeftAsync();
    ShowMessage("Get Country Drive On Left", countriesDriveLeft, action);

    var northCapital = await server.GetCaptialOnNorthHemisphereAsync();    
    ShowMessage("Get Capital On North Hemisphere", northCapital, action);

    WriteLine("Get Country By Domain");
    var result = await server.GetCountryByDomainAsync("https://www.yle.fi");
    WriteLine($"Result: {result.Name.Common}");

    var closeTime = await server.GetCloseTimesAsync();
    ShowMessage("Get Country Close Time To Finland", closeTime, action);

    List<(string name, string countryCode)> people = new List<(string name, string countryCode)>
    {
       ("Messi", "Argentina"),
       ("Linus Torvald", "Finland"),
    };
    await server.PrintPeopleInfoAsync(people);
  }
}