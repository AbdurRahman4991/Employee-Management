namespace MyFirstApi.Models;

public class OrganizationUnit
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Level { get; set; } = null!;

    public int? ParentId { get; set; }

    // Parent
    public OrganizationUnit? Parent { get; set; }

    // Children
    public ICollection<OrganizationUnit> Children { get; set; }
        = new List<OrganizationUnit>();

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}