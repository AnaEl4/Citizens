using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public IActionResult Post([FromBody] CreateCitizenRequest citizenToAdd)
    {
        Citizen existingCitizen = _citizenList.Find(c => c.CI == citizenToAdd.CI);

        if (existingCitizen != null)
        {
            return Ok($"Citizen with CI {citizenToAdd.CI} already exists");
        }
        var random = new Random();
        string bloodGroup = _bloodGroups[random.Next(_bloodGroups.Length)];

        Citizen newCitizen = new Citizen
        {
            CI = citizenToAdd.CI,
            FirstName = citizenToAdd.FirstName,
            LastName = citizenToAdd.LastName,
            BloodGroup = bloodGroup,
            PersonalAsset = "default"
        };

        _citizenList.Add(newCitizen);
        SaveCitizensToCsv();

        return Ok(newCitizen);
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_citizenList);
    }

    [HttpGet]
    [Route("{ci}")]
    public IActionResult Get([FromRoute] int ci)
    {
        Citizen foundCitizen = _citizenList.Find(c => c.CI == ci);

        if (foundCitizen == null)
        {
            return Ok($"Citizen with CI {ci} not found");
        }

        return Ok(foundCitizen);
    }

    [HttpPut]
    [Route("{ci}")]
    public IActionResult Put([FromRoute] int ci, [FromBody] UpdateCitizenRequest request)
    {
        Citizen citizenToUpdate = _citizenList.Find(c => c.CI == ci);

        if (citizenToUpdate == null)
        {
            return Ok($"Citizen with CI {ci} not found");
        }

        citizenToUpdate.FirstName = request.FirstName;
        citizenToUpdate.LastName = request.LastName;

        SaveCitizensToCsv();

        return Ok(citizenToUpdate);
    }

    [HttpDelete]
    [Route("{ci}")]
    public IActionResult Delete([FromRoute] int ci)
    {
        Citizen citizenToRemove = _citizenList.Find(c => c.CI == ci);

        if (citizenToRemove == null)
        {
            return Ok($"Citizen with CI {ci} not found");
        }

        _citizenList.Remove(citizenToRemove);
        SaveCitizensToCsv();

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

    //Funcion auxiliar para signar grupo sanguineo
    private string[] _bloodGroups = new[]
    {
        "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"
    };
}
