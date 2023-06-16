using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

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

    [Params(1000,10000)]
    public int Iterations { get; set; }
    [GlobalSetup]
    public void Setup()
    {
        input = string.Concat(Enumerable.Repeat("This is an example sentence", Iterations));
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