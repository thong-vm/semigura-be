namespace GraphQL
{
    public static class MyExtensions
    {
        public static string LowerFirstChar(this string txt)
        {
            return string.Concat(txt[..1].ToLower(), txt.AsSpan(1));
        }

    }
}
