//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Running;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using SpanBenchmark.Data;
//using SpanBenchmark.Data.Model;
//using SpanBenchmark.Data.Services;
//using System.Text.Json;

//public class Program
//{
//    static void Main(string[] args)
//    {
//        var services = new ServiceCollection();
//        services.AddDbContext<ApplicationDbContext>(options =>
//                             options.UseNpgsql("Host=localhost;Database=eCampus_DB_TEST;Username=postgres;password=admin"));

//        IServiceProvider serviceProvider = services.BuildServiceProvider();
//        var ctx = serviceProvider.GetService<ApplicationDbContext>();


//        // var userService = new UserService();
//        //userService.GetEmployees().GetAwaiter().GetResult();
//        //userService.BulkUpsertUserDefault().GetAwaiter().GetResult();
//        BenchmarkRunner.Run<EmployeeBench>();
//        Console.ReadLine();
//    }
//}

//[MemoryDiagnoser]
//public class EmployeeBench
//{
//    private readonly UserService _userService;

//    public EmployeeBench()
//    {
//        //var services = new ServiceCollection();
//        //IServiceProvider serviceProvider = services.BuildServiceProvider();
//        //var ctx = serviceProvider.GetService<ApplicationDbContext>();

//        string employeesString = "";
//        try
//        {
//            StreamReader sr = new StreamReader("C:\\Users\\akdog\\source\\repos\\hakanakdogan\\SpanBenchmark\\SpanBenchmark\\Test.txt");
//            employeesString = sr.ReadLine();
//            sr.Close();
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine("Exception: " + e.Message);
//        }
//        finally
//        {
//            Console.WriteLine("Executing finally block.");
//        }
//        var employeesList = JsonSerializer.Deserialize<List<JsonDocument>>(employeesString);
//        _userService = new UserService(employeesList);
//    }

//    //    [GlobalSetup]
//    //    public void Setup()
//    //    {
//    //        input = string.Concat(Enumerable.Repeat("This is an example sentence", Iterations));
//    //        _data = Enumerable.Repeat("This is an example sentence", Iterations).ToList();
//    //        _queue = new Queue<string>();
//    //        for (int i = 0; i < Iterations; i++)
//    //        {
//    //            _queue.Enqueue("This is an example sentence");
//    //        }
//    //        _stack = new Stack<string>();
//    //        for (int i = 0; i < Iterations; i++)
//    //        {
//    //            _stack.Push("This is an example sentence");
//    //        }

//    //    }


//    //[Benchmark]
//    //public async Task<bool> BenchDefault()
//    //{
//    //   await  _userService.BulkUpsertUserDefault();
//    //    return true;
//    //}

//    //[Benchmark]
//    //public async Task<bool> BenchSpan()
//    //{
//    //    await _userService.BulkUpsertUserSpan();
//    //    return true;
//    //}

//    [Benchmark]
//    public void SpanReplace()
//    {
//        _userService.SpanReplace();
//    }

//    [Benchmark]
//    public void ListReplace()
//    {
//        _userService.ListReplace();
//    }

//}



//[MemoryDiagnoser]
//[Config(typeof(Config))]
//public class Bench
//{
//    public class Config : ManualConfig
//    {
//        public Config()
//        {
//            var config = ManualConfig.CreateEmpty()
//                .AddColumnProvider(DefaultColumnProviders.Instance)
//                .AddLogger(ConsoleLogger.Default);

//            var exporter = new CsvExporter(
//                CsvSeparator.CurrentCulture,
//                new SummaryStyle(
//                    cultureInfo: System.Globalization.CultureInfo.CurrentCulture,
//                    printUnitsInHeader: true,
//                    printUnitsInContent: false,
//                    timeUnit: Perfolizer.Horology.TimeUnit.Millisecond,
//                    sizeUnit: SizeUnit.KB

//                    ));
//            config.AddExporter(exporter);
//        }


//    }
//    private string input;
//    private readonly string searchTerm = "example";
//    private readonly string replaceTerm = "sample";
//    private readonly string startsWith = "This";

//    private ReadOnlySpan<char> InputSpan => input.AsSpan();
//    private ReadOnlySpan<char> SearchTermSpan => searchTerm.AsSpan();
//    private ReadOnlySpan<char> ReplacetermSpan => replaceTerm.AsSpan();
//    private ReadOnlySpan<char> StarsWithSpan => startsWith.AsSpan();
//    private List<string> _data;
//    private Queue<string> _queue;
//    private Stack<string> _stack;

//    [Params(100)]
//    public int Iterations { get; set; }
//    [GlobalSetup]
//    public void Setup()
//    {
//        input = string.Concat(Enumerable.Repeat("This is an example sentence", Iterations));
//        _data = Enumerable.Repeat("This is an example sentence", Iterations).ToList();
//        _queue = new Queue<string>();
//        for (int i = 0; i < Iterations; i++)
//        {
//            _queue.Enqueue("This is an example sentence");
//        }
//        _stack = new Stack<string>();
//        for (int i = 0; i < Iterations; i++)
//        {
//            _stack.Push("This is an example sentence");
//        }

//    }


//    [Benchmark]
//    public void StringReplace() => input.Replace(searchTerm, replaceTerm);
//    [Benchmark]
//    public void SpanReplace() => ReplaceSpan(InputSpan, SearchTermSpan, ReplacetermSpan);

//    [Benchmark]
//    public string StringSubString() => input.Substring(0, 20);

//    [Benchmark]
//    public ReadOnlySpan<char> SpanSlice() => InputSpan.Slice(0, 20);

//    [Benchmark]
//    public bool StringStartsWith() => input.StartsWith(startsWith);

//    [Benchmark]
//    public bool SpanStartsWith() => InputSpan.StartsWith(StarsWithSpan);

//    [Benchmark]
//    public bool ListContains() => _data.Contains("example");
//    [Benchmark]
//    public bool SpanContains() => CollectionsMarshal.AsSpan(_data).Contains("example");
//    [Benchmark]
//    public bool QueueContains() => _queue.Contains("example");
//    [Benchmark]
//    public bool StackContains() => _stack.Contains("example");



//    private static void ReplaceSpan(ReadOnlySpan<char> source, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
//    {
//        int indexOfSearch = source.IndexOf(oldValue);
//        int resultLength = source.Length + (newValue.Length - oldValue.Length);
//        Span<char> result = stackalloc char[resultLength];
//        source.Slice(0, indexOfSearch).CopyTo(result);
//        newValue.CopyTo(result.Slice(indexOfSearch));
//        source.Slice(indexOfSearch + oldValue.Length).CopyTo(result.Slice(indexOfSearch + newValue.Length));
//    }
//}
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using CommandLine.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;

public class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<Bench>();
    }
}




[MemoryDiagnoser]
[Config(typeof(Config))]
public class Bench
{
    public class Config : ManualConfig
    {
        public Config()
        {
            var config = ManualConfig.CreateEmpty()
                .AddColumnProvider(DefaultColumnProviders.Instance)
                .AddLogger(ConsoleLogger.Default);

            var exporter = new CsvExporter(
                CsvSeparator.CurrentCulture,
                new SummaryStyle(
                    cultureInfo: System.Globalization.CultureInfo.CurrentCulture,
                    printUnitsInHeader: true,
                    printUnitsInContent: false,
                    timeUnit: Perfolizer.Horology.TimeUnit.Millisecond,
                    sizeUnit: SizeUnit.KB

                    ));
            config.AddExporter(exporter);
        }


    }
    private string input;
    private readonly string searchTerm = "example";
    private readonly string replaceTerm = "sample";
    private readonly string startsWith = "This";

    private ReadOnlySpan<char> InputSpan => input.AsSpan();
    private ReadOnlySpan<char> SearchTermSpan => searchTerm.AsSpan();
    private ReadOnlySpan<char> ReplacetermSpan => replaceTerm.AsSpan();
    private ReadOnlySpan<char> StarsWithSpan => startsWith.AsSpan();
    private List<string> _data;
    private Queue<string> _queue;
    private Stack<string> _stack;

    private List<string> _listUsers;


    [Params(10000)]
    public int Iterations { get; set; }
    [GlobalSetup]
    public void Setup()
    {
        string employeesString = "";
        try
        {
            StreamReader sr = new StreamReader("C:\\Users\\akdog\\source\\repos\\hakanakdogan\\SpanBenchmark\\SpanBenchmark\\Test.txt");
            employeesString = sr.ReadLine();
            sr.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        finally
        {
            Console.WriteLine("Executing finally block.");
        }
        var employeesList = JsonSerializer.Deserialize<List<JsonDocument>>(employeesString);
        foreach (var employee in employeesList)
        {
            var modifiedJson = JsonDocument.Parse(SetServiceDataProperty(employee).ToJsonString());
            var username = modifiedJson.RootElement.GetProperty("title").GetString();
            _listUsers = new List<string>();
            if(username != null)
            {
                _listUsers.Add(username);
            }
            
        }
        input = string.Concat(Enumerable.Repeat("This is an example sentence", Iterations));
        _data = Enumerable.Repeat("This is an example sentence", Iterations).ToList();
        _queue = new Queue<string>();
        for (int i = 0; i < Iterations; i++)
        {
            _queue.Enqueue("This is an example sentence");
        }
        _stack = new Stack<string>();
        for (int i = 0; i < Iterations; i++)
        {
            _stack.Push("This is an example sentence");
        }

    }


    //[Benchmark]
    //public void StringReplace() => input.Replace(searchTerm, replaceTerm);
    //[Benchmark]
    //public void SpanReplace() => ReplaceSpan(InputSpan, SearchTermSpan, ReplacetermSpan);

    //[Benchmark]
    //public string StringSubString() => input.Substring(0, 20);

    //[Benchmark]
    //public ReadOnlySpan<char> SpanSlice() => InputSpan.Slice(0, 20);

    //[Benchmark]
    //public bool StringStartsWith() => input.StartsWith(startsWith);

    //[Benchmark]
    //public bool SpanStartsWith() => InputSpan.StartsWith(StarsWithSpan);

    //[Benchmark]
    //public bool ListContains() => _data.Contains("example");
    //[Benchmark]
    //public bool SpanContains() => CollectionsMarshal.AsSpan(_data).Contains("example");
    //[Benchmark]
    //public bool QueueContains() => _queue.Contains("example");
    //[Benchmark]
    //public bool StackContains() => _stack.Contains("example");

    [Benchmark]
    public bool ListRealDataContains() => _listUsers.Contains("Isten");

    [Benchmark]
    public bool SpanRealDataContains() => CollectionsMarshal.AsSpan(_data).Contains("Isten");
    [Benchmark]
    public void ListRealDataBinarySearch() => _listUsers.BinarySearch("Isten");
    [Benchmark]
    public void SpanRealDataBinarySearch() => CollectionsMarshal.AsSpan(_data).BinarySearch("Isten");
    [Benchmark]
    public void ListRealDataBinarySlice() => _listUsers.GetRange(0, (_listUsers.Count - 1));
    [Benchmark]
    public void SpanRealDataBinarySlice() => CollectionsMarshal.AsSpan(_data).Slice(0, (_listUsers.Count - 1));



    private static void ReplaceSpan(ReadOnlySpan<char> source, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
    {
        int indexOfSearch = source.IndexOf(oldValue);
        int resultLength = source.Length + (newValue.Length - oldValue.Length);
        Span<char> result = stackalloc char[resultLength];
        source.Slice(0, indexOfSearch).CopyTo(result);
        newValue.CopyTo(result.Slice(indexOfSearch));
        source.Slice(indexOfSearch + oldValue.Length).CopyTo(result.Slice(indexOfSearch + newValue.Length));
    }
    private JsonNode SetServiceDataProperty(JsonDocument employee)
    {
        var rawJson = JsonNode.Parse(employee.RootElement.GetRawText());
        if (!employee.RootElement.TryGetProperty("isServiceData", out _))
            rawJson.AsObject().Add("isServiceData", true);
        else
            rawJson["isServiceData"] = true;
        return rawJson;
    }
}