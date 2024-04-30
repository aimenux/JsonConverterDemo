namespace App.Launchers;

public interface ILauncher
{
    string Name { get; }
    
    string[] DependsOn { get; }

    void Launch();
}