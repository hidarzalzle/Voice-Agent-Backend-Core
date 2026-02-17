namespace VoiceAgent.Domain.ValueObjects;

public readonly record struct CallId(string Value)
{
    public static CallId New() => new(Guid.NewGuid().ToString("N"));

    public override string ToString() => Value;
}
