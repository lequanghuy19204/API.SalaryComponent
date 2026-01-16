namespace Core.SalaryComponent.DTOs;

public class MoveMultipleResultDto
{
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> SkippedCodes { get; set; } = new List<string>();
}

public class MoveMultipleRequestDto
{
    public List<Guid> Ids { get; set; } = new List<Guid>();
}
