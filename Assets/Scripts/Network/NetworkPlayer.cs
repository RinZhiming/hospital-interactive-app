using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class NetworkPlayer : NetworkBehaviour
{
    [Networked] public ref NetworkPlayerId NetworkPlayerId => ref MakeRef<NetworkPlayerId>();
    protected FirebaseAuth auth;
    protected bool isSpawn;
    private Vector3 random;

    public override async void Spawned()
    {
        if (!HasStateAuthority) return;
        
        auth = FirebaseAuth.DefaultInstance;

        random = RandomVector(new Vector3(0, 0, 0), new Vector3(-3, 0, -2));
        transform.position = random;

        await UniTask.WaitForSeconds(0.01f);
        
        isSpawn = true;
    }

    private Vector3 RandomVector(Vector3 min, Vector3 max)
    {
        var x = Random.Range(min.x, max.x);
        var y = Random.Range(min.y, max.y);
        var z = Random.Range(min.z, max.z);
        return new Vector3(x, y, z);
    }
}