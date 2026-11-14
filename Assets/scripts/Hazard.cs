using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damage;
    public float xRebound = 1f;
    public float yRebound = 1f;

    // Called when something with a rigidBody stays inside something that has a collider (collider has to have 'is Trigger' box ticked)
    void OnTriggerStay2D(Collider2D collisionObject) // collisionObject is the object that collided with whatever this script is attached to
    {
        // Only accepts components which are PlayerController scripts. Gets the PlayerController script from the object that collided
        PlayerController controller = collisionObject.GetComponent<PlayerController>();
        if (controller != null) {
            controller.ChangeHealth(-damage);        
            Rigidbody2D rigidBody = collisionObject.GetComponent<Rigidbody2D>();
            bool positive = rigidBody.linearVelocity.x > 0;
            float xOffset = xRebound;
            if (positive){
                xOffset = -xRebound;
            }
            rigidBody.linearVelocity = new Vector2(xOffset, yRebound);
        }
    }
}