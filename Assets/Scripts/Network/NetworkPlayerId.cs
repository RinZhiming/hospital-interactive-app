using Fusion;

public struct NetworkPlayerId : INetworkStruct
{
    public NetworkString<_64> playerId;
    
    [Networked] 
    [Capacity(64)]
    [UnityMultiline]
    public string PlayerId {get => default; set { } }
}