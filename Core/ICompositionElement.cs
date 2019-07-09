namespace MEFLight
{
    public interface ICompositionElement
    {
        string DisplayName { get; }

        ICompositionElement Origin { get; }

    }
}
