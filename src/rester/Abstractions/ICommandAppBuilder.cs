using System.CommandLine;

namespace rester.Abstractions;

public interface ICommandAppBuilder
{
    ICommandAppBuilder AddOption<T>(string name, string description);
    ICommandAppBuilder AddOption<T>(string name, string alias, string description);
    ICommandAppBuilder AddOptionWithDefault<T>(string name, string description, T defaultValue);
    ICommandAppBuilder AddOptionAllowMultipleArguments<T>(string name, string alias, string description);
    ICommandAppBuilder AddArgument<T>(string name, string description);
    ICommandAppBuilder SetAction(Func<ParseResult, CancellationToken, Task> action);
    RootCommand Create();
}