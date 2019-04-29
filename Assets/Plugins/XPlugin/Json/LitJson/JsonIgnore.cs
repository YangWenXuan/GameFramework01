using System;

namespace XPlugin.Data.Json {
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class JsonIgnoreAttribute : Attribute {

	}
}