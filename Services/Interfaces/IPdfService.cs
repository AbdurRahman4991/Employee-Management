namespace MyFirstApi.Services.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateEmployeePdf(
            List<MyFirstApi.Models.Employee> employees);
    }
}