namespace seesharp.moqingbirt.TestBench.NewNameSpace
{
    using System.Globalization;

    public interface ISpecifiedServiceInOtherNamespace : IParentInterface
    {
        string MyName { get; }

        string SayMyName(CultureInfo language);
    }
}