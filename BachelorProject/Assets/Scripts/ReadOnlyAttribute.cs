using UnityEngine;

// [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
/// <summary>
/// Allow to display an attribute in inspector without allow editing
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute { }