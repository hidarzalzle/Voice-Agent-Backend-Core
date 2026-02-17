namespace VoiceAgent.Domain.ValueObjects;

public enum SessionState
{
    Initiated,
    Ringing,
    Connected,
    Listening,
    Processing,
    Responding,
    WaitingForTool,
    Completed,
    Failed
}
