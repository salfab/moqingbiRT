namespace seesharp.moqingbirt.TestBench
{
    public interface IMyInjectedService
    {
        bool IsAvailable { get; set; }

        int ReturnAnInteger();

        void SetAnInteger(double myDouble);
    }
}