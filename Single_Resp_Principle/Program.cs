public class Product
{
    public string Name { get; set; } = "";
    public int Price { get; set; }
}

// результат валидации 
public class ValidationResult
{
    public List<string> Errors { get; } = new List<string>();
    public bool IsValid => Errors.Count == 0;

    public void Add(string error) => Errors.Add(error);
    public void AddRange(IEnumerable<string> errors) => Errors.AddRange(errors);
}

public interface IProductValidator
{
    ValidationResult Validate(Product product);
}

public class DefaultProductValidator : IProductValidator
{
    public ValidationResult Validate(Product p)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(p.Name) || p.Name.Length < 2)
            result.Add("Name must be at least 3 characters.");

        if (p.Price <= 0)
            result.Add("Price must be greater than zero.");

        return result;
    }
}

public class CustomerServiceProductValidator : IProductValidator
{
    public ValidationResult Validate(Product p)
    {
        var result = new ValidationResult();
        if (p.Price <= 100_000)
            result.Add("For CustomerService the price must be > 100000.");
        return result;
    }
}

// простая агрегация валидаторов
public class CompositeProductValidator : IProductValidator
{
    private readonly IList<IProductValidator> _validators;
    public CompositeProductValidator(IEnumerable<IProductValidator> validators)
    {
        _validators = new List<IProductValidator>(validators);
    }

    public ValidationResult Validate(Product p)
    {
        var combined = new ValidationResult();
        foreach (var v in _validators)
        {
            var r = v.Validate(p);
            combined.AddRange(r.Errors);
        }
        return combined;
    }
}

class Program
{
    static void Main()
    {
        var product = new Product { Name = "TV", Price = 5000000 };

        // обычная валидация
        IProductValidator defaultValidator = new DefaultProductValidator();
        var res1 = defaultValidator.Validate(product);
        Console.WriteLine("Default valid: " + res1.IsValid);
        if (!res1.IsValid) Console.WriteLine(string.Join("; ", res1.Errors));

        // валидация для CustomerService
        IProductValidator csValidator = new CustomerServiceProductValidator();
        var res2 = csValidator.Validate(product);
        Console.WriteLine("CustomerService valid: " + res2.IsValid);
        if (!res2.IsValid) Console.WriteLine(string.Join("; ", res2.Errors));

        // запустить все правила сразу
        var composite = new CompositeProductValidator(new[] { defaultValidator, csValidator });
        var resAll = composite.Validate(product);
        Console.WriteLine("Combined valid: " + resAll.IsValid);
        if (!resAll.IsValid) Console.WriteLine(string.Join("; ", resAll.Errors));
    }
}
