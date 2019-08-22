 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    
    private Vector3 touchStart;
    private Vector2 screenBound;    
    private GameController gameController;
    public GameObject followCamera;
    private float defaultCameraSize;

    // Start is called before the first frame update
    void Start()
    {            
        Camera.main.orthographicSize = gameController.ScreenHeight()/2 ;    
        defaultCameraSize = gameController.ScreenHeight()/2;
    }

    // Update is called once per frame
    void Update()
    {   
        float boundary = gameController.boundaries();
        float screenHeight = Screen.height;
        float screenWidth  = Screen.width;
        
        if(Input.GetMouseButtonDown(0)){
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }else if(Input.touchCount == 2){
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne  = Input.GetTouch(1);

            Vector2  touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2  touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            float different =  currentMagnitude - prevMagnitude;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - different*0.4f,100.0f,defaultCameraSize);
            // handleZoom(different *0.4f);

        }else if(Input.GetMouseButton(0)){
            Vector3 direction1 = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);   
            Vector3 direction2 = direction1;         
            direction1.x = Mathf.Clamp(followCamera.transform.position.x + direction1.x,boundary*-1f/2+screenWidth/2,boundary*1f/2 -screenWidth/2);
            direction1.y = Mathf.Clamp(followCamera.transform.position.y + direction1.y,boundary*-1f/2+screenHeight/2, boundary*1f/2 -screenHeight/4 );        
            direction2.x = Mathf.Clamp(Camera.main.transform.position.x + direction2.x, 0,boundary*1f);
            direction2.y = Mathf.Clamp(Camera.main.transform.position.y + direction2.y,boundary*-1f/2+ screenHeight/2 ,boundary*1f);
            direction2.z = -10;      

            if(direction1.x > direction2.x) direction1.x = direction2.x;
            else direction2.x = direction1.x;
            if(direction1.y > direction2.y) direction1.y = direction2.y;
            else direction2.y = direction1.y;
            direction1.y  += Screen.height/2;

            followCamera.transform.position = direction1;
            Camera.main.transform.position = direction2;
            
        }
    }

    public void SetGameControllerReference (GameController controller){
        gameController = controller;
    }

    private void handleZoom(float increment){
        // float zoomChangeAmount = 80f;
        // if(Input.GetAxis("Mouse ScrollWheel")>0){
        //     if(zoom1 >5) zoom1 -= zoomChangeAmount * Time.deltaTime *10f;
        // }
        // if(Input.GetAxis("Mouse ScrollWheel")<0){
        //     if(zoom1 < 4900) zoom1 += zoomChangeAmount * Time.deltaTime *10f;
        // }
        // GetComponent<Camera>().orthographicSize = zoom1;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment,100.0f,1000.0f);
    }

    private void handleMove(){
        // if (Input.GetMouseButtonDown(0))
        // {
        //     dragOrigin = Input.mousePosition;
        //     return;
        // }
 
        // if (!Input.GetMouseButton(0)) return;
 
        // Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        // Vector3 move = new Vector3(pos.x * dragSpeed,  pos.y * dragSpeed,0);
 
        // transform.Translate(move, Space.World);  
    }
}
