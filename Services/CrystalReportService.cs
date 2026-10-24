using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public class CrystalReportService : IPdfService
{
    public byte[] GenerateEmployeePdf(
        List<Employee> employees)
    {
        ReportDocument report = new ReportDocument();

        try
        {
            report.Load("EmployeeReport.rpt");

            report.SetDataSource(employees);

            using var stream = report.ExportToStream(
                ExportFormatType.PortableDocFormat
            );

            using var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }
        finally
        {
            report.Close();
            report.Dispose();
        }
    }
}