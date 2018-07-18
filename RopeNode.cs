using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeNode : MonoBehaviour {

    public Transform player;

    public Object node_prefab;

    public LayerMask corner_layer; // layer mask for corner colliders
    public Transform attached_object;
    public GameObject child_node; // next node in chain

    private Transform last_corner = null; // the last corner collider that was collided with by this node's rope
    private bool can_split_at_last_corner_again = false; // can only split again at the last corner it split at after it has moved away from the corner collider (prevents it retriggering)

    private float previous_angle_with_child = 0; // keeps track of angle between node and child node directions. If it changes sign then they must be rejoined
    private bool can_combine = false; // when initiated it is not possible to join with its child. This will become true when it has a child and the angle between them is increasing


    // Use this for initialization
    void Awake ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        node_prefab = Resources.Load("RopeNode");
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector2 node_to_child = NodeToChild();

        RaycastHit2D hit_info; // to store rayscast info
            
        // first check if rope has collided with a corner. If so we need to split the rope
        hit_info = Physics2D.Raycast(transform.position, node_to_child, node_to_child.magnitude, corner_layer.value);
        if (hit_info.collider != null)
        {
            if (hit_info.transform != last_corner || can_split_at_last_corner_again)
            {
                last_corner = hit_info.transform;
                SplitRope(hit_info.transform);
            }
        }
        else if (last_corner)
        {
            can_split_at_last_corner_again = true;
        }
        // if the node has a child we need to check if it should rejoin with the child
        if (child_node != null)
        {
            // if it is allowed to combine with child then we check if it should be
            if (can_combine)
            {
                float current_angle_with_child = AngleWithChild();
                if (Mathf.Sign(previous_angle_with_child) != Mathf.Sign(current_angle_with_child))
                {
                    JoinWithChild();
                }
            }
            // if it cant be combined yet, we check if can_join should be set to true
            else
            {
                // if the magnitude of the angle with the child is increasing then set can_combine to true
                if( Mathf.Abs( AngleWithChild() ) > Mathf.Abs(previous_angle_with_child) )
                {
                    can_combine = true;
                }
            }
        }
        // update the previous angle with child (not this is in separate if to above as the child may have been destroyed)
        if (child_node != null)
        {
            previous_angle_with_child = AngleWithChild();
        }
    }

    // split the rope at specified point
    void SplitRope(Transform corner)
    {
        Vector2 node_to_split = (Vector2)corner.position - (Vector2)transform.position; // vector from this node to the split point
        // create and set up child node
        child_node = (GameObject)Instantiate(node_prefab, corner.position, Quaternion.identity, transform);

        // Declare corner as the transform the node is attached to
        child_node.GetComponent<RopeNode>().attached_object = corner;

        // child rope connects to the rigid body this node is attached to. distance is current distance - current node to corner
        child_node.GetComponent<DistanceJoint2D>().connectedBody = GetComponent<DistanceJoint2D>().connectedBody;
        child_node.GetComponent<DistanceJoint2D>().distance = GetComponent<DistanceJoint2D>().distance - node_to_split.magnitude;

        // disable the joint on this node
        GetComponent<DistanceJoint2D>().enabled = false;

        // update combination and splitting flags. After splitting, the node must wait for certain conditions before being able to combine with its child or to split at the same corner again
        // to combine: the angle between the node and its child must have grown
        // to split again: there must have been a raycast to the child where the last corner wasnt seen
        can_split_at_last_corner_again = false;
        can_combine = false;

        // update previous angle with child (otherwise will be 0 initially which can cause problems)
        previous_angle_with_child = AngleWithChild();
    }

    // joins the rope with the child
    public void JoinWithChild()
    {
        if (child_node != null)
        {
            // if child node is the last node, i.e connected to the player, then connect this node to the player
            if(child_node.GetComponent<RopeNode>().child_node == null)
            {
                GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
                Destroy(child_node);
                child_node = null;
            }
            // otherwise connect the the child of the child
            else
            {
                GameObject grandchild_node = child_node.GetComponent<RopeNode>().child_node;
                Destroy(child_node);
                child_node = grandchild_node;
            }
            // re-enable the joint
            GetComponent<DistanceJoint2D>().enabled = true;
        }
    }

    // short function to make it easier to understand and look cleaner
    public float AngleWithChild()
    {
        Vector2 v1 = NodeToChild();
        Vector2 v2 = child_node.GetComponent<RopeNode>().NodeToChild();
        float sign = Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
        return sign * Vector2.Angle(v1, v2);
    }

    // returns vector to child node or to the player if it has none
    public Vector2 NodeToChild()
    {
        Vector2 node_to_child;
        if (child_node)
        {
            node_to_child = child_node.transform.position - transform.position;
        }
        else
        {
            node_to_child = player.transform.position - transform.position;
        }
        return node_to_child;
    }
}
