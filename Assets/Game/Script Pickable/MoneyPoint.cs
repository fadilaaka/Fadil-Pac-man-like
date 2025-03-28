using UnityEngine;

public class MoneyPoint : MonoBehaviour
{
    public int value = 1;
    private bool isCollected = false;

    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;
        GameManager.Instance.AddScore(value);
        Destroy(gameObject);
    }
}
