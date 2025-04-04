namespace IdentityService.BLL.DTOs;

public record FreelancerSkillDto
{
    public FreelancerSkillDto()
    {
    }

    public FreelancerSkillDto(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; init; }
    public string Name { get; init; }
}