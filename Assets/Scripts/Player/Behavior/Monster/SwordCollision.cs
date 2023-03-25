using Player.Behaviour.Monster;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    [SerializeField] private MonsterBehaviour monsterBehaviour;

    private void OnTriggerEnter(Collider collision)
    {
        monsterBehaviour.OnSwordCollider(collision.gameObject);
    }
}
