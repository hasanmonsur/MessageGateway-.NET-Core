namespace SmsGatewaySystem.Helpers
{
    public static class SliceCL
    {
        public static List<T> Slice<T>(this IEnumerable<T> source, int startIndex, int endIndex)
        {
            //DONE: validation
            if (null == source)
                throw new ArgumentNullException("source");
            else if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex",
                  $"startIndex ({startIndex}) must be non-negative.");
            else if (startIndex > endIndex)
                throw new ArgumentOutOfRangeException("startIndex",
                  $"startIndex ({startIndex}) must not exceed endIndex ({endIndex}).");

            // Instead of pure Linq 
            // return source.Skip(startIndex).Take(endIndex - startIndex).ToList();
            // let's use a loop

            // it doesn't create endIndex - startIndex items but reserve space for them
            List<T> result = new List<T>(endIndex - startIndex);

            foreach (var item in source.Skip(startIndex))
                if (result.Count >= endIndex - startIndex)
                    return result;
                else
                    result.Add(item); // we add items, not put them with result[i] = ...

            // source exhausted, but we don't reach endIndex. 
            //TODO: Shall we return as is or throw an exception?
            return result;
        }
    }
}
