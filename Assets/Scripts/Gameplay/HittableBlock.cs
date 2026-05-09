using UnityEngine;

public class HittableBlock : MonoBehaviour
{
    public void OnHitFromBelow()
    {
        Debug.Log($"Hit: {gameObject.name}");
    }
}
