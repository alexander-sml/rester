namespace rester.Abstractions;

public interface ICommandRunner
{
    Task<int> ExecuteAsync(string[] args);
}