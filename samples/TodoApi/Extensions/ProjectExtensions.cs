using System.Linq;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.Extensions
{
  public static class ProjectExtensions
  {
    public static ProjectDto ToDto(this Project prj)
    {
      return new ProjectDto
      {
        Id = prj.Id,
        Name = prj.Name,
        Tasks = prj.Tasks.Select(t => t.ToDto()).ToList()
      };
    }
  }
}
