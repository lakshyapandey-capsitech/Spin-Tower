using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public List<GameObject> stackElements = new List<GameObject>();
    public Material safeMat;
    public Material dangerMat;

    // need 3 elements, Element 1 used for blank zone, Elem2: for death zone, Elem3: for safe zone

    public void GenerateRandomStack(bool isFirstObstacle = false)
    {
        // Elem1: Blank(Empty) element 
        // dangerElem1 and dangerElem2 are variables used for targetting death zones on a disc
        if (isFirstObstacle)
        {
            foreach (var g in stackElements)
            {
                g.GetComponent<Renderer>().material = safeMat;
                g.tag = "Safe";
            }
            stackElements[5].SetActive(false);
        }
        else
        {
            int totalElemsTillNow = (int)GameManager.Instance.playerScore / 10;
            int difficultyLevel = totalElemsTillNow / 3;
            int totalDangerElemCnt;

            if (difficultyLevel == 0 || difficultyLevel == 1)
            {
                totalDangerElemCnt = Random.Range(0, 2);
            }
            else if (difficultyLevel == 2)
            {
                totalDangerElemCnt = Random.Range(1, 4);
            }
            else
            {
                totalDangerElemCnt = Random.Range(2, 5);
            }

            // randomly choose any part of the disc to be empty
            int emptyElemIdx = Random.Range(0, stackElements.Count);

            // randomly choose the danger zones
            List<int> dangerElemIndices = new List<int>();
            for (int i = 0; i < totalDangerElemCnt; i++)
            {
                int foundIdx;
                do
                {
                    foundIdx = Random.Range(0, stackElements.Count);
                }

                // skip those which are empty and already danger zone
                while (foundIdx == emptyElemIdx || dangerElemIndices.Contains(foundIdx));
                {
                    dangerElemIndices.Add(foundIdx);
                }
            }

            // resetting all elements to safe
            foreach (var g in stackElements)
            {
                g.GetComponent<Renderer>().material = safeMat;
                g.tag = "Safe";
            }

            // keep that element to be empty
            stackElements[emptyElemIdx].SetActive(false);

            // make the elements danger
            foreach (int index in dangerElemIndices)
            {
                stackElements[index].GetComponent<Renderer>().material = dangerMat;
                stackElements[index].tag = "Danger";
            }
        }

    }
}
