using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Word : MonoBehaviour
{
    private enum wordKinds
    {
        NON=-1,
        HEAD,
        ELEMENT,
        TAIL,
        MAX
    }

    void Start()
    {
        // menuActive_ = menuActive;
        // menuActive_ = GameObject.Find("Mng").GetComponent<MenuActive>();
        gameObject.SetActive(true);

        //StartCoroutine(ActiveWord(menuActive));

    }
    public IEnumerator ActiveWord(MenuActive menuActive)
    {
        gameObject.SetActive(true);
        while (true)
        {
            yield return null;
            if (menuActive.GetStringNumber() != (int)MenuActive.topic.WORD)
            {
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (menuActive_.StringNumber() == (int)MenuActive.topic.WORD)
        //{
        //    gameObject.SetActive(true);
        //}
        //else
        //{
        //    gameObject.SetActive(false);
        //}

    }
}
