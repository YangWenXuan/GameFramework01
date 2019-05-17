using System;

namespace Com.TheFallenGames.OSA.Core
{
	[Obsolete("It was renamed to OSA. Use that instead and then run the migration tool under frame8->OSA->Migration", true)]
	public abstract class SRIA<TParams, TItemViewsHolder> : OSA<TParams, TItemViewsHolder> 
		where TParams : BaseParams
		where TItemViewsHolder : BaseItemViewsHolder
	{ }
}
