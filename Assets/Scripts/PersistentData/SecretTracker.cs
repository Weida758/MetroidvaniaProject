using UnityEngine;
using System.Collections.Generic;

public class SecretTracker
{
    public HashSet<string> secrets { get; private set; } = new HashSet<string>();

    

    public void CollectSecret(string id)
    {
        secrets.Add(id);
    }
    
    public bool ContainsSecret(string id)
    {
        return secrets.Contains(id);
    }


}
