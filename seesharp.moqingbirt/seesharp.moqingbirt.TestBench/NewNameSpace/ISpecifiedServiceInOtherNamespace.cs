namespace seesharp.moqingbirt.TestBench.NewNameSpace
{
    using System.Globalization;

    public interface ISpecifiedServiceInOtherNamespace
    {
        string MyName { get; }

        string SayMyName(CultureInfo language);
    }
}