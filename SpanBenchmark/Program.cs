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
    string[] products = new string[500];
    public Bench()
    {
        Random rnd = new Random();
        
        for (int i = 0; i < products.Length; i++)
        {
            int num = rnd.Next(1, 100);
            string name = GetRandomProductName(rnd);
            products[i] = $"{num} {name}";
        }
        static string GetRandomProductName(Random rnd)
        {
            string[] names = { "Laptop", "Televizyon", "Buzdolabı", "Kulaklık", "Monitör", "Klavye", "Fare", "Cep Telefonu", "Kamera", "Hoparlör" };
            int index = rnd.Next(names.Length);
            return names[index];
        }
    }

    [Benchmark]
    public int SpanCalculate()
    {
        var prods = products.AsSpan();
        var sum = 0;
        foreach (var item in prods)
        {
            var count = Int32.Parse( item.Split(' ')[0]);
            sum+= count;
        }

        return sum;
    }
    [Benchmark]
    public int ListCalculate()
    {
        var prods = products.ToList();
        var sum = 0;
        foreach (var item in prods)
        {
            var count = Int32.Parse(item.Split(' ')[0]);
            sum += count;
        }

        return sum;
    }

}