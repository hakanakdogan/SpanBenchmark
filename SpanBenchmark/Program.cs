using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
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
    public string StringReplace() => input.Replace(searchTerm, replaceTerm);
    [Benchmark]
    public string SpanReplace()
    {
        return ReplaceSpan(InputSpan, SearchTermSpan, ReplacetermSpan).ToString();
    }
    [Benchmark]
    public string StringSubString() => input[..20];

    [Benchmark]
    public string SpanSlice() => InputSpan[..20].ToString();

    [Benchmark]
    public bool StringStartsWith() => input.StartsWith(startsWith);

    [Benchmark]
    public bool SpanStartsWith() => InputSpan.StartsWith(StarsWithSpan);
    [Benchmark]
    public bool TestString() => input.Equals(searchTerm);
    [Benchmark]
    public bool TestSpan() => MemoryExtensions.Equals(InputSpan, SearchTermSpan, StringComparison.Ordinal);

    [Benchmark]
    public bool ListContains() => _data.Contains("fsdgasd");
    [Benchmark]
    public bool SpanContains()
    {
        return CollectionsMarshal.AsSpan(_data).Contains("fsdgasd");
    }
    [Benchmark]
    public bool ArrayListContains() => _dataArray.Contains(_data);

    [Benchmark]
    public bool QueueContains() => _queue.Contains("fsdgasd");

    [Benchmark]
    public bool StackContains() => _stack.Contains("fsdgasd");



    private static ReadOnlySpan<char> ReplaceSpan(ReadOnlySpan<char> source, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
    {
        int index = source.IndexOf(oldValue);
        if (index == -1) return source;
        char[] result = new char[source.Length + newValue.Length - oldValue.Length];
        Span<char> resultSpan = result;

        source.Slice(0, index).CopyTo(resultSpan);
        newValue.CopyTo(resultSpan.Slice(index));
        source.Slice(index+oldValue.Length).CopyTo(resultSpan.Slice((index+oldValue.Length)/2));
        return resultSpan;
    }
}