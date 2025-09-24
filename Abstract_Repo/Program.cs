using System;
using System.Collections.Generic;

// Абстрактный базовый класс для всех сущностей
public abstract class AbstractEntity
{
    public int Id { get; set; }
}

// Конкретные реализации сущностей
public class AccountEntity : AbstractEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
}

public class RoleEntity : AbstractEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public interface IRepository
{
    void Save(AbstractEntity entity);
}

// Конкретные реализации репозиториев
public class AccountRepository : IRepository
{
    public void Save(AbstractEntity entity)
    {
        // Приведение типа и логика для Account
        var account = entity as AccountEntity;
        if (account == null)
            throw new ArgumentException("Expected AccountEntity type");
        
        Console.WriteLine($"Saving account: {account.Username}, {account.Email}");
    }
}

public class RoleRepository : IRepository
{
    public void Save(AbstractEntity entity)
    {
        // Приведение типа и логика для Role
        var role = entity as RoleEntity;
        if (role == null)
            throw new ArgumentException("Expected RoleEntity type");
        
        Console.WriteLine($"Saving role: {role.Name}, {role.Description}");
    }
}

// Фабрика для создания репозиториев
public class RepositoryFactory
{
    private readonly Dictionary<Type, IRepository> _repositories = new Dictionary<Type, IRepository>();

    public RepositoryFactory()
    {
        _repositories[typeof(AccountEntity)] = new AccountRepository();
        _repositories[typeof(RoleEntity)] = new RoleRepository();
    }

    public IRepository GetRepository(Type entityType)
    {
        if (_repositories.TryGetValue(entityType, out var repository))
            return repository;
        
        throw new NotSupportedException($"No repository registered for type {entityType.Name}");
    }
}

class Program
{
    static void Main()
    {
        // Создаем фабрику репозиториев
        var repositoryFactory = new RepositoryFactory();
        
        var account = new AccountEntity 
        { 
            Id = 1, 
            Username = "john_doe", 
            Email = "john@example.com" 
        };
        
        var role = new RoleEntity 
        { 
            Id = 1, 
            Name = "Admin", 
            Description = "Administrator role" 
        };
        
        // Сохраняем обьекты через соответствующие репозитории
        var accountRepository = repositoryFactory.GetRepository(typeof(AccountEntity));
        accountRepository.Save(account);
        
        var roleRepository = repositoryFactory.GetRepository(typeof(RoleEntity));
        roleRepository.Save(role);
        
        Console.WriteLine("Done!");
    }
}