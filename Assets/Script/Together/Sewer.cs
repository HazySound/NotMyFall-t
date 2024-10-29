using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sewer : MonoBehaviour
{
    [SerializeField]
    private GameObject[] FallObjectPrefabs;

    [SerializeField]
    private Transform Position_1;
    [SerializeField]
    private Transform Position_2;
    [SerializeField]
    private Transform Position_3;

    public void SpawnObject()
    {
        GameObject.Instantiate(FallObjectPrefabs[0], Position_1);//포지션 1에 랜덤 스폰
        GameObject.Instantiate(FallObjectPrefabs[1], Position_2);//포지션 2에 랜덤 스폰
        GameObject.Instantiate(FallObjectPrefabs[2], Position_3);//포지션 3에 랜덤 스폰
    }
    public void ClearObject()
	{
        if(Position_1.childCount > 0)
		{
            Position_1.GetChild(0).gameObject.SetActive(false);
            Destroy(Position_1.GetChild(0).gameObject);
            Position_2.GetChild(0).gameObject.SetActive(false);
            Destroy(Position_2.GetChild(0).gameObject);
            Position_3.GetChild(0).gameObject.SetActive(false);
            Destroy(Position_3.GetChild(0).gameObject);
        }
    }
}
