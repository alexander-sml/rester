using System.CommandLine;
using rester.Abstractions;

namespace rester;

public class CommandAppBuilder : ICommandAppBuilder
{
    private readonly RootCommand _rootCommand;

    public CommandAppBuilder(string name)
    {
        _rootCommand = new RootCommand(name);
    }
    
    public ICommandAppBuilder AddOption<T>(string name, string description)
    {
        var option = new Option<T>(name)
        {
            Description = description
        };
        
        _rootCommand.Options.Add(option);

        return this;
    }

    public ICommandAppBuilder AddOption<T>(string name, string alias, string description)
    {
        var option = new Option<T>(name, alias)
        {
            Description = description
        };
        
        _rootCommand.Options.Add(option);

        return this;
    }
    
    public ICommandAppBuilder AddOptionWithDefault<T>(string name, string description, T defaultValue)
    {
        var option = new Option<T>(name)
        {
            Description = description,
            DefaultValueFactory = _ => defaultValue
        };
        
        _rootCommand.Options.Add(option);

        return this;
    }
    
    public ICommandAppBuilder AddOptionAllowMultipleArguments<T>(string name, string alias, string description)
    {
        var option = new Option<T>(name, alias)
        {
            Description = description,
            AllowMultipleArgumentsPerToken = true
        };
        
        _rootCommand.Options.Add(option);

        return this;
    }
    
    public ICommandAppBuilder AddArgument<T>(string name, string description)
    {
        var argument = new Argument<T>(name)
        {
            Description = description
        };
        
        _rootCommand.Arguments.Add(argument);

        return this;
    }

    public ICommandAppBuilder SetAction(Func<ParseResult, CancellationToken, Task> action)
    {
        _rootCommand.SetAction(action);
        
        return this;
    }

    public RootCommand Create()
    {
        return _rootCommand;
    }
}