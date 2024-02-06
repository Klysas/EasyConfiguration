namespace EasyConfiguration.Core.Extensions
{
	internal static class IEnumerableExtensions
	{
		internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source is null) throw new ArgumentNullException(nameof(source));
			if (action is null) throw new ArgumentNullException(nameof(action));

			foreach (T element in source)
				action(element);
		}
	}
}