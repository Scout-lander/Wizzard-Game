using UnityEngine;

public class ChargingEnemyMovement : EnemyMovement
{

    Vector2 chargeDirection;

    protected override void Start()
    {
        base.Start();
        chargeDirection = (player.transform.position - transform.position).normalized;
    }

    public override void Move()
    {
        transform.position += (Vector3)chargeDirection * enemy.ActualStats.moveSpeed * Time.deltaTime;
    }
}
