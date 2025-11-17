using System;
using System.Collections.Generic;
using UnityEngine;

public class EXO1 : MonoBehaviour
{

    [SerializeField] GameObject cube;
    [SerializeField] float radius = 0.5f;
    [SerializeField] List<compo> cubes;
    [SerializeField] int maxDepth = 4;

    [System.Serializable]
    public struct compo
    {
        public Vector3 position;
        public int div;
        public GameObject obj;
    }

    void Start()
    {
        GameObject first=Instantiate(cube, new Vector3(0, 0, 0), Quaternion.identity);
        compo firstCompo = new compo();
        firstCompo.position = first.transform.position;
        firstCompo.div = 1;
        firstCompo.obj = first;
        cubes = new List<compo>();
        cubes.Add(firstCompo);
        CreateSphere();
    }

    void CreateSphere()
    {
        int depth = 0;
        Vector3 center = Vector3.zero;
        while (depth < maxDepth) {
            List<compo> currentCubes = new List<compo>(cubes);
            for (int i = 0; i < currentCubes.Count; i++) {
                Vector3 closest = currentCubes[i].position - center;
                closest = closest.normalized * radius;
                compo current = currentCubes[i];
                if(Vector3.Distance(center,current.position) < radius)
                {
                    Divide(i);
                } else if (Mathf.Abs(closest.x - current.position.x) < 1f / current.div && Mathf.Abs(closest.y - current.position.y) < 1f / current.div && Mathf.Abs(closest.z - current.position.z) < 1f / current.div)
                {
                    Divide(i);
                }
                else
                {
                    Destroy(cubes[i].obj);
                    cubes.RemoveAt(i);
                }

            }
            depth++;
        }
    }


    void Divide(int index)
    {
        Vector3 position = cubes[index].position;
        int oldDiv = cubes[index].div;
        if (oldDiv == 0) oldDiv = 1;

        GameObject oldObj = cubes[index].obj;

        GameObject[] newCubes = new GameObject[8];

        float scaleFactor = 1f / (oldDiv + 1);

        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    Vector3 offset = new Vector3(x * scaleFactor, y * scaleFactor, z * scaleFactor) * cube.transform.localScale.x * scaleFactor;
                    Vector3 newPos = position + offset;

                    GameObject inst = Instantiate(cube, newPos, Quaternion.identity);
                    inst.transform.localScale = cube.transform.localScale * scaleFactor;

                    newCubes[ToIndex(x, y, z)] = inst;
                }
            }
        }

        Destroy(oldObj);
        cubes.RemoveAt(index);

        for (int i = 0; i < 8; i++)
        {
            compo newCompo = new compo();
            newCompo.position = newCubes[i].transform.position;
            newCompo.div = oldDiv + 1;
            newCompo.obj = newCubes[i];
            cubes.Add(newCompo);
        }
    }



    int ToIndex(int x, int y, int z)
    {
        int xi = (x == 1) ? 1 : 0;
        int yi = (y == 1) ? 1 : 0;
        int zi = (z == 1) ? 1 : 0;

        return (xi << 2) | (yi << 1) | zi;
    }

}
