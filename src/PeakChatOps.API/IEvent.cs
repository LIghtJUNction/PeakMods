using System;

namespace PeakChatOps.API;

public interface IEvent
{
}

[AttributeUsage(AttributeTargets.Class)]
public class EventNameAttribute : Attribute
{
    public string Name { get; }
    public EventNameAttribute(string name) => Name = name;
}
[AttributeUsage(AttributeTargets.Class)]
public class EventDescriptionAttribute : Attribute
{
    public string Description { get; }
    public EventDescriptionAttribute(string description) => Description = description;
}