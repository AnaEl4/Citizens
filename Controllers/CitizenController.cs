using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/citizen")]
public class CitizenController: ControllerBase
{
    // Funciones de acuerdo al CRUD

    //Create
    [HttpPost]
    public void Post (){}
    
    //Read ALL/ Retrieve ALL
    [HttpGet]
    public void Get (){}

    //Read by CI / Retrieve by CI
    [HttpGet]
    [Route("{ci}")]
    public void Get (int ci){}
    
    //Update
    [HttpPut]
    [Route("{ci}")]
    public void Put (int ci){}
    
    //Delete
    [HttpDelete]
    [Route("{ci}")]
    public void Delete (int ci){}

}
