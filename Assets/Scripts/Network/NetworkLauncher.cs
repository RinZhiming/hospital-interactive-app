using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private List<NetworkPlayer> players = new();
    private NetworkRunner networkRunner;
    private NetworkSceneManagerDefault scenenManager;

    public async void OnEnterRoom(GameMode mode, string roomName, int sceneIndex)
    {
        var runner = gameObject.AddComponent<NetworkRunner>();
        var scenemanager = gameObject.AddComponent<NetworkSceneManagerDefault>();
        
        scenenManager = scenemanager;
        var scene = new NetworkSceneInfo();
        scene.AddSceneRef(SceneRef.FromIndex(sceneIndex));
        
        await runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = roomName,
            SceneManager = scenemanager,
            Scene = scene,
            PlayerCount = 20,
        });
    }

    public void OnLeftRoom()
    {
        networkRunner.Shutdown();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            networkRunner = runner;
            runner.Spawn(players[CharacterSelectManager.CharacterIndex], 
                new Vector3(-3, 0, -2),
                Quaternion.identity, 
                player, 
                (networkrunner, o) =>
            {
                networkrunner.SetPlayerObject(player, o);
            });
            
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            LoadManager.Loading(false);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            runner.Despawn(runner.GetPlayerObject(player));
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Destroy(networkRunner);
        Destroy(scenenManager);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }
}