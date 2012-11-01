namespace seesharp.moqingbirt.TestBench
{
    using System;

    public interface IMyInjectedService
    {
        bool IsAvailable { get; set; }

        int ReturnAnInteger();

        void SetAnInteger(double myDouble);

        Guid SetAnOtherInteger(double myDouble);

        Guid PassTwoDoubles(double param1, double param2);
    }
}