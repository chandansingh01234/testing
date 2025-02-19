using UnityEngine;

public class SoldierSpawner : MonoBehaviour
{
    public GameObject attackerPrefab;
    public GameObject defenderPrefab;
    public EnergyManager energyManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("PlayerLand"))
                {
                    // Spawn Attacker using Player's energy
                    if (energyManager.SpendEnergy(true, 2)) // Player spends 2 energy
                    {
                        SpawnSoldier(attackerPrefab, hit.point);
                    }
                }
                else if (hit.collider.CompareTag("EnemyLand"))
                {
                    // Spawn Defender using Enemy's energy
                    if (energyManager.SpendEnergy(false, 3)) // Enemy spends 3 energy
                    {
                        SpawnSoldier(defenderPrefab, hit.point);
                    }
                }
            }
        }
    }

    void SpawnSoldier(GameObject prefab, Vector3 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }
}