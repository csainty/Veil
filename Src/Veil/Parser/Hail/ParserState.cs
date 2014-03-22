namespace Veil.Parser.Hail
{
    internal enum ParserState
    {
        ReadingContent,
        PossibleIdentifier,
        ReadingIdentifier,
        EndingIdentifier
    }
}