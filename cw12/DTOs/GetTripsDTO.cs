namespace cw12.DTOs;

public class GetTripsDTO
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    
    public List<GetCountriesDTO> Countries { get; set; }
    public List<GetClientDTO> Clients { get; set; }
}

public class GetCountriesDTO
{
    public string Name { get; set; }
}

public class GetClientDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}