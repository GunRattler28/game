using UnityEngine;

public class cameraFollowPrototype : MonoBehaviour
{
    public  float FollowSpeed = 2f;
    public Transform target;


    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x,target.position.y,-8f);
        transform.position = Vector3.Slerp(transform.position,newPos,FollowSpeed*Time.deltaTime);
    }
}
