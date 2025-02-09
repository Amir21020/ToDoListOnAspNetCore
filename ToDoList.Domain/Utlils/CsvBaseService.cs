using CsvHelper;
using CsvHelper.Configuration;
using System.Collections;
using System.Globalization;
using System.Text;

namespace ToDoList.Domain.Utlils;

public sealed class CsvBaseService<T>
{
    private readonly CsvConfiguration _csvConfiguration;
    public CsvBaseService()
    {
        _csvConfiguration = GetConfiguration();
    }
    
    public CsvConfiguration GetConfiguration()
    {
        return new CsvConfiguration(cultureInfo:
            CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = Encoding.UTF8,
            NewLine = "\r\n"
        };
    }
    public byte[] UploadFile(IEnumerable data)
    {
        using var memory = new MemoryStream();
        using var streaWriter = new StreamWriter(memory);
        using var csvWriter = new CsvWriter(streaWriter, _csvConfiguration);
        csvWriter.WriteRecords(data);
        streaWriter.Flush();
        return memory.ToArray();
    }
}
