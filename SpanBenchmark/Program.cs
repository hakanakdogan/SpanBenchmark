using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using CommandLine.Text;
using System.Collections;
using System.Runtime.InteropServices;

public class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<Bench>();
    }
}


[MemoryDiagnoser]
public class Bench
{
    private string input;
    private readonly string searchTerm = "example";
    private readonly string replaceTerm = "sample";
    private readonly string startsWith = "This";

    private ReadOnlySpan<char> InputSpan => input.AsSpan();
    private ReadOnlySpan<char> SearchTermSpan => searchTerm.AsSpan();
    private ReadOnlySpan<char> ReplacetermSpan => replaceTerm.AsSpan();
    private ReadOnlySpan<char> StarsWithSpan => startsWith.AsSpan();
    private List<string> _data;
    private ArrayList _dataArray;
    private Queue<string> _queue;
    private Stack<string> _stack;

    [Params(1000,10000)]
    public int Iterations { get; set; }
    [GlobalSetup]
    public void Setup()
    {
        input = string.Concat(Enumerable.Repeat("This is an example sentence", Iterations));
        _data = Enumerable.Repeat("This is an example sentence", Iterations).ToList();
        _dataArray = new ArrayList();
        Enumerable.Repeat(_dataArray.Add("This is an example sentence"), Iterations);
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
    [Benchmark]
    public void StringReplace() => input.Replace(searchTerm, replaceTerm);
    [Benchmark]
    public void SpanReplace() => ReplaceSpan(InputSpan, SearchTermSpan, ReplacetermSpan);
    
    //[Benchmark]
    //public string StringSubString() => input[..20];

    //[Benchmark]
    //public ReadOnlySpan<char> SpanSlice() => InputSpan[..20];

    //[Benchmark]
    //public bool StringStartsWith() => input.StartsWith(startsWith);

    //[Benchmark]
    //public bool SpanStartsWith() => InputSpan.StartsWith(StarsWithSpan);
    //[Benchmark]
    //public bool TestString() => input.Equals(searchTerm);
    //[Benchmark]
    //public bool TestSpan() => MemoryExtensions.Equals(InputSpan, SearchTermSpan, StringComparison.Ordinal);

    //[Benchmark]
    //public bool ListContains() => _data.Contains("example");
    //[Benchmark]
    //public bool SpanContains() => CollectionsMarshal.AsSpan(_data).Contains("example");

    //[Benchmark]
    //public bool ArrayListContains() => _dataArray.Contains("example");

    //[Benchmark]
    //public bool QueueContains() => _queue.Contains("example");

    //[Benchmark]
    //public bool StackContains() => _stack.Contains("example");



    private static void ReplaceSpan(ReadOnlySpan<char> source, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
    {
        int indexOfSearch = source.IndexOf(oldValue);
        int resultLength = source.Length + (newValue.Length - oldValue.Length);
        Span<char> result = stackalloc char[resultLength];
        source.Slice(0, indexOfSearch).CopyTo(result);
        newValue.CopyTo(result.Slice(indexOfSearch));
        source.Slice(indexOfSearch + oldValue.Length).CopyTo(result.Slice(indexOfSearch + newValue.Length));
    }
}