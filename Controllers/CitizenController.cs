using Microsoft.AspNetCore.Mvc;
using Serilog;

[ApiController]
[Route("api/citizen")]
public class CitizenController : ControllerBase
{
    private List<Citizen> _citizenList;
    private IConfiguration _configuration;


    public CitizenController(IConfiguration configuration)
    {
        _citizenList = new List<Citizen>();
        _configuration = configuration;


        List<string[]> data = CSVHelper.ReadCSV(_configuration["Data:Location"]);

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].Length >= 5)
            {
                Citizen citizen = new Citizen
                {
                    CI = int.Parse(data[i][0]),
                    FirstName = data[i][1],
                    LastName = data[i][2],
                    BloodGroup = data[i][3],
                    PersonalAsset = data[i][4]
                };

                _citizenList.Add(citizen);
            }
        }
    }

    //FUNCIONES CRUD

    //Create
    [HttpPost]
    public IActionResult Post([FromBody] CreateCitizenRequest citizenToAdd)
    {
        Citizen existingCitizen = _citizenList.Find(c => c.CI == citizenToAdd.CI);

        if (existingCitizen != null)
        {
            return Ok($"Citizen with CI {citizenToAdd.CI} already exists");
        }
        
         //LISTA DE GRUPOS SANGUINEOS
        string[] _bloodGroups = new[]
        {
            "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"
        };

        //Asignar grupo sanguineo aleatorio
        Random random = new Random();
        string randomBloodGroup = _bloodGroups[random.Next(_bloodGroups.Length)];

        //Asignar objeto aleatorio de api externa
        ObjectService objectService = new ObjectService(_configuration);
        string randomObjectName = objectService.GetRandomObjectName().Result;

        Citizen newCitizen = new Citizen
        {
            CI = citizenToAdd.CI,
            FirstName = citizenToAdd.FirstName,
            LastName = citizenToAdd.LastName,
            BloodGroup = randomBloodGroup,
            PersonalAsset = randomObjectName
        };

        _citizenList.Add(newCitizen);
        SaveCitizensToCsv();

        Log.Information($"New citizen created: {newCitizen.CI} - {newCitizen.FirstName} - {newCitizen.LastName} - {newCitizen.BloodGroup} - {newCitizen.PersonalAsset}");
        return Ok(newCitizen);
    }


    //Read/Retrieve ALL
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_citizenList);
    }


    //Read/Retrieve by CI
    [HttpGet]
    [Route("{ci}")]
    public IActionResult Get([FromRoute] int ci)
    {
        Citizen foundCitizen = _citizenList.Find(c => c.CI == ci);

        if (foundCitizen == null)
        {
            Log.Error($"Citizen with CI {ci} not found");
            return Ok($"Citizen with CI {ci} not found");
        }

        return Ok(foundCitizen);
    }


    //Update --> by CI (only name and last name can be updated)
    [HttpPut]
    [Route("{ci}")]
    public IActionResult Put([FromRoute] int ci, [FromBody] UpdateCitizenRequest request)
    {
        Citizen citizenToUpdate = _citizenList.Find(c => c.CI == ci);

        if (citizenToUpdate == null)
        {
            Log.Error($"Citizen to update with CI {ci} not found");
            return Ok($"Citizen with CI {ci} not found");
        }

        citizenToUpdate.FirstName = request.FirstName;
        citizenToUpdate.LastName = request.LastName;


        SaveCitizensToCsv();

        Log.Information($"Citizen with CI: {citizenToUpdate.CI} UPDATED");

        return Ok(citizenToUpdate);
    }


    //Delete --> by CI
    [HttpDelete]
    [Route("{ci}")]
    public IActionResult Delete([FromRoute] int ci)
    {
        Citizen citizenToRemove = _citizenList.Find(c => c.CI == ci);

        if (citizenToRemove == null)
        {
            Log.Error($"Citizen to delete with CI {ci} not found");
            return Ok($"Citizen with CI {ci} not found");
        }

        _citizenList.Remove(citizenToRemove);
        SaveCitizensToCsv();
   
        Log.Information($"Citizen with CI: {citizenToRemove.CI} DELETED");
        
        return Ok("Citizen deleted successfully");
    }


    //Funcion auxiliar para guardar en csv
    private void SaveCitizensToCsv()
    {
        List<string[]> data = _citizenList.Select(c => new string[]
        {
            c.CI.ToString(),
            c.FirstName,
            c.LastName,
            c.BloodGroup,
            c.PersonalAsset
        }).ToList();

        CSVHelper.WriteCSV(_configuration["Data:Location"], data);
    }


}
