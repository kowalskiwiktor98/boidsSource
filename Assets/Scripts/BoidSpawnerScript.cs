using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class BoidSpawnerScript : MonoBehaviour
{
    public GameObject BoidPrefab;
    public Slider slider;

    [Range(0, 300)]
    public int BoidCount = 10;

    private List<GameObject> BoidList = new List<GameObject>();

    public float xMaxOffset;
    public float yMaxOffset;

    private void Start()
    {
        for (int i = 0; i < BoidCount; i++)
        {
            SpawnBoid();
        }
    }

    public void Update()
    {
        BoidCount = (int) slider.value;
        int currentCount = BoidList.Count;
        if (currentCount < BoidCount) SpawnBoid();
        if (currentCount > BoidCount)
        {
            GameObject toDestroy = BoidList[Random.Range(0, BoidCount - 1)];
            Destroy(toDestroy);
            BoidList.Remove(toDestroy);
        }
    }

    private void SpawnBoid()
    {
        BoidList.Add(Instantiate(BoidPrefab, new Vector3(Random.Range(-xMaxOffset, xMaxOffset), Random.Range(-yMaxOffset, yMaxOffset), 0), Quaternion.identity));
    }

    public void ChangeBoidAmount(float value)
    {
        BoidCount = (int)value;
    }
}