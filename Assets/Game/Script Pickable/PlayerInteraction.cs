using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 2.75f;
    public LayerMask obstacleLayers;
    private MoneyPoint nearbyMoneyPoint;

    void Update()
    {
        DetectNearbyMoneyPoint();
        if (nearbyMoneyPoint != null && Input.GetKeyDown(KeyCode.E))
        {
            if (CanInteractWithMoneyPoint(nearbyMoneyPoint))
            {

                nearbyMoneyPoint.Collect();
                nearbyMoneyPoint = null;
            }
        }
    }

    void DetectNearbyMoneyPoint()
    {
        MoneyPoint[] allMoneyPoints = FindObjectsByType<MoneyPoint>(FindObjectsSortMode.None);
        nearbyMoneyPoint = null;

        float closestDistance = interactionDistance;

        foreach (var money in allMoneyPoints)
        {
            float distance = Vector3.Distance(transform.position, money.transform.position);
            if (distance <= closestDistance)
            {
                closestDistance = distance;
                nearbyMoneyPoint = money;
            }
        }
    }

    bool CanInteractWithMoneyPoint(MoneyPoint moneyPoint)
    {
        Vector3 direction = (moneyPoint.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, moneyPoint.transform.position);
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, obstacleLayers))
        {
            if (hit.collider.gameObject != moneyPoint.gameObject)
            {
                return false;
            }
        }
        return true;
    }
}
