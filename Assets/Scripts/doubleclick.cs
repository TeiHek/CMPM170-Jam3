using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class doubleclick : MonoBehaviour
{
    public UnityEvent DoubleClick;
    public Transform units;
    private float firstLeftClickTime;
    private float timeBetweenLeftClick = 0.5f;
    private bool isTimeCheckAllowed = true;
    private int leftClickNum = 0;
    private bool ontheunit = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(1))
        {
            leftClickNum += 1;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit)){
                if(hit.transform.name == units.name){
                    ontheunit = true;
                }
            }
        }
        if(leftClickNum == 1 && isTimeCheckAllowed && ontheunit )
        {
            firstLeftClickTime = Time.time;
            StartCoroutine(DetectDoubleLeftClick());
        }
    }

    private IEnumerator DetectDoubleLeftClick()
    {
        isTimeCheckAllowed = false;
        while(Time.time < firstLeftClickTime + timeBetweenLeftClick)
        {
            if(leftClickNum == 2)
            {
                Debug.Log("Double Click!");
                Click2();
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        leftClickNum = 0;
        isTimeCheckAllowed = true;
    }

    private void Click2(){
        DoubleClick.Invoke();
    }
}
