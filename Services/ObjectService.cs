using Newtonsoft.Json;

public class ObjectService
{
    private HttpClient _httpClient;

    public ObjectService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(configuration["ExternalServices:ObjectsApi:BaseUrl"]);
    }

    public async Task<string> GetRandomObjectName()
    {

        //Si la api funciona y se logra obtener un objeto, se devuelve el objeto sino se devuelve "objeto invalido"
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error retrieving objects from external API");
            }

            string responseContent = await response.Content.ReadAsStringAsync();

            List<Object>? objects = JsonConvert.DeserializeObject<List<Object>>(responseContent);

            if (objects == null || objects.Count == 0)
            {
                throw new Exception("No objects were returned by the external API");
            }

            Random random = new Random();
            Object randomObject = objects[random.Next(objects.Count)];

            return randomObject.name;
        }
        catch(Exception)
        {
            return "Objeto Inválido";
        }
    }
}
