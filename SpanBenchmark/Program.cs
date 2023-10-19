﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpanBenchmark.Data;
using SpanBenchmark.Data.Services;


public class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
                             options.UseNpgsql("Host=localhost;Database=eCampus_DB_TEST;Username=postgres;password=admin"));

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        var ctx = serviceProvider.GetService<ApplicationDbContext>();
        

        //var userService = new UserService(ctx);
        //userService.GetEmployees().GetAwaiter().GetResult();
        BenchmarkRunner.Run<EmployeeBench>();
        Console.ReadLine();
    }
}

[MemoryDiagnoser]
public class EmployeeBench
{
    private readonly UserService _userService;

    public EmployeeBench()
    {
        var services = new ServiceCollection();
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        var ctx = serviceProvider.GetService<ApplicationDbContext>();
        _userService = new UserService(ctx);
    }

    [Benchmark]
    public async Task<bool> BenchDefault()
    {
       await  _userService.BulkUpsertUserDefault();
        return true;
    }

    [Benchmark]
    public async Task<bool> BenchSpan()
    {
        await _userService.BulkUpsertUserSpan();
        return true;
    }

}



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