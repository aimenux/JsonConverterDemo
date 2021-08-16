namespace App.Launchers
{
    public interface ILauncher
    {
        string Name { get; }

        void Launch();
    }
}
