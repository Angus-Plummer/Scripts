using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeNode : MonoBehaviour {

    public Transform player;
    public Object node_prefab;

    public LayerMask obstacle_layer; // layer mask for obstacles
    public Transform attached_object;

    public int attached_vertex; // index of the vertex of the polygon collider this node is attached to
    public float vertex_offset; // the distance that the rope nodes will form from the vertex of the attached obstacle

    public GameObject child_node; // next node in chain

    public float length; // length of this segment of rope


    // Use this for initialization
    void Awake ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        node_prefab = Resources.Load("Prefabs/Rope Node");
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePosition();
        Vector2 node_to_child = NodeToChild();
        GetComponent<DistanceJoint2D>().distance = length;

        RaycastHit2D hit_info; // to store rayscast info
            
        // first check if rope has collided with a corner. If so we need to split the rope
        hit_info = Physics2D.Raycast(transform.position, node_to_child, node_to_child.magnitude, obstacle_layer.value);
        if (hit_info.collider != null)
        {
            // convert the coordinates of the vertices to world coords
            Vector2[] vertex_world_positions = new Vector2[hit_info.transform.GetComponent<PolygonCollider2D>().points.Length];
            for (int i = 0; i < hit_info.transform.GetComponent<PolygonCollider2D>().points.Length; i++)
            {
                vertex_world_positions[i] = hit_info.transform.TransformPoint(hit_info.transform.GetComponent<PolygonCollider2D>().points[i]);
            }
            // determine if any vertices need to be excluded when finding nearest vertex to collision
            int excluding_vertex = -1; // define as -1 as not sure what undefined will do
            if (hit_info.transform == attached_object) // if the collided object is the one this node is attached to then cant split at the current position
            {
                excluding_vertex = attached_vertex;
            }
            else if (child_node) // if there is a child node then cant split again at the child's position
            {
                if (hit_info.transform == child_node.GetComponent<RopeNode>().attached_object)
                {
                    excluding_vertex = child_node.GetComponent<RopeNode>().attached_vertex;
                }
            }
            // find the vertex on the collider that is nearest the line and split there
            int vertex_index = ClosestToLine(transform.position, (Vector2)transform.position + node_to_child, vertex_world_positions, excluding_vertex);
            SplitRope(hit_info.transform, vertex_index);
  
        }
       
        // if the node has a child we need to check if it should rejoin with the child
        if (child_node != null)
        {
            // the two rope segments should rejoin if a triangle shaped collider with corners of this node, child, and grandchild does not intersect with the obstacle whos corner the child of this node belongs to

            // first update the position of the node, its child, and its grandchild
            UpdatePosition();
            child_node.GetComponent<RopeNode>().UpdatePosition();
            if (child_node.GetComponent<RopeNode>().child_node)
            {
                child_node.GetComponent<RopeNode>().child_node.GetComponent<RopeNode>().UpdatePosition();
            }
            Vector2 node_to_grandchild = NodeToChild() + child_node.GetComponent<RopeNode>().NodeToChild();
            Vector2[] points = new Vector2[3];
            points[0] = Vector2.zero;
            points[1] = NodeToChild();
            points[2] = node_to_grandchild;
            GetComponent<PolygonCollider2D>().points = points;
            GetComponent<PolygonCollider2D>().enabled = true;
            bool overlapping = ArePolygonsOverlapped(GetComponent<PolygonCollider2D>(), child_node.GetComponent<RopeNode>().attached_object.GetComponent<PolygonCollider2D>());
            GetComponent<PolygonCollider2D>().enabled = false;
            if (!overlapping)
            {
                JoinWithChild();
            }
        }

        MaintainLength();
        GetComponent<DistanceJoint2D>().distance = length;
        UpdatePosition();
    }

    // split the rope at specified point
    void SplitRope(Transform obstacle, int vertex_index)
    {
        Vector2 corner_position = obstacle.TransformPoint(obstacle.GetComponent<PolygonCollider2D>().points[vertex_index]);
        Vector2 node_to_split = corner_position - (Vector2)transform.position; // vector from this node to the split point
        GameObject current_child = child_node;
        // create and set up child node
        child_node = (GameObject)Instantiate(node_prefab, corner_position, Quaternion.identity, transform);
        if (current_child)
        {
            current_child.transform.SetParent(child_node.transform);
            child_node.GetComponent<RopeNode>().child_node = current_child;
            child_node.GetComponent<DistanceJoint2D>().enabled = false;
        }

        // set the attached obstacle and vertex of the new node
        child_node.GetComponent<RopeNode>().attached_object = obstacle;
        child_node.GetComponent<RopeNode>().attached_vertex = vertex_index;
        // update the position to give correct offset from the vertex
        child_node.GetComponent<RopeNode>().UpdatePosition();

        // child rope connects to the rigid body this node is attached to. distance is current distance - current node to corner
        child_node.GetComponent<DistanceJoint2D>().connectedBody = GetComponent<DistanceJoint2D>().connectedBody;
        child_node.GetComponent<RopeNode>().length = length - node_to_split.magnitude;
        child_node.GetComponent<DistanceJoint2D>().distance = child_node.GetComponent<RopeNode>().length;

        // disable the joint on this node but connect to the new child
        GetComponent<DistanceJoint2D>().enabled = false;
        GetComponent<DistanceJoint2D>().connectedBody = child_node.GetComponent<Rigidbody2D>();
        length = node_to_split.magnitude;
    }

    // joins the rope with the child
    public void JoinWithChild()
    {
        if (child_node != null)
        {
            length += child_node.GetComponent<RopeNode>().length;
            // if child node is the last node, i.e connected to the player, then connect this node to the player
            if (child_node.GetComponent<RopeNode>().child_node == null)
            {
                GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
                Destroy(child_node);
                child_node = null;
                // re-enable the joint
                GetComponent<DistanceJoint2D>().enabled = true;
            }
            // otherwise connect the the child of the child
            else
            {
                GameObject grandchild_node = child_node.GetComponent<RopeNode>().child_node;
                grandchild_node.transform.SetParent(transform);
                Destroy(child_node);
                child_node = grandchild_node;

            }
        }
    }

    // function to update the position of the rope node to the attached vertex of the attached object or the hook if it is the first node
    // the position used is offset from the vertex by a small amount to make collision detection with the attached object easier
    public void UpdatePosition()
    {
        if (attached_object == transform.parent) // if the attached object is the parent (i.e. if it is the hook the update position to the hook position)
        {
            transform.position = transform.parent.position;
        }
        else // if attached object is an obstacle then update the position to the attached vertex on the obstacle
        {
            PolygonCollider2D attached_collider = attached_object.GetComponent<PolygonCollider2D>();
            int next_vertex_index = attached_vertex + 1;
            int previous_vertex_index = attached_vertex - 1;

            // wrap the next and previous index around if they are out of bounds
            if (next_vertex_index == attached_collider.points.Length)
            {
                next_vertex_index -= attached_collider.points.Length;
            }
            if (previous_vertex_index == -1)
            {
                previous_vertex_index += attached_collider.points.Length;
            }
            // create vectors of the two edges
            Vector2 edge1 = attached_collider.points[attached_vertex] - attached_collider.points[previous_vertex_index];
            Vector2 edge2 = attached_collider.points[attached_vertex] - attached_collider.points[next_vertex_index];

            Vector2 vertex_normal = (edge1.normalized + edge2.normalized).normalized;

            Vector2 new_position = (Vector2)attached_object.TransformPoint(attached_collider.points[attached_vertex]) + (Vector2)attached_object.TransformDirection(vertex_normal).normalized * vertex_offset;
            transform.position = new_position;
        }
    }

    // maintain length of the rope. will take or give length to the child node as appropriate to maintain the length
    public void MaintainLength()
    {
        // if there is a child node then exchange length with it
        if (child_node)
        {
            float target_length = length;
            float actual_length = NodeToChild().magnitude;
            float delta = actual_length - target_length;
            length = actual_length;

            float child_target_length = child_node.GetComponent<RopeNode>().length;
            if (child_target_length > delta)
            {
                child_node.GetComponent<RopeNode>().length -= delta;
            }
            else
            {
                if (child_node.GetComponent<RopeNode>().child_node == null)
                {
                    Destroy(player.GetComponent<BallHandler>().current_hook);
                    player.GetComponent<BallHandler>().current_hook = null;
                }
                else
                {
                    float missing = delta - child_target_length;
                    length -= missing;
                    child_node.GetComponent<RopeNode>().length = 0;
                    JoinWithChild();
                }
            }
        }
        // if there is no child node (i.e. this node is attached to the player) then shorten the rope to the distance the player is from the node
        else
        {
            float current_separation = NodeToChild().magnitude;
            if(current_separation < length)
            {
                length = current_separation;
            }
        }
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

    // find the index of the points in "points[]" which is closest to a line. Excliding a specified index
    public int ClosestToLine(Vector2 line_start, Vector2 line_end, Vector2[] points, int index_excluding)
    {
        int index_of_closest = 0;
        float distance_to_closest = Mathf.Infinity;
        for (int i = 0; i < points.Length; i++)
        {
            if (i != index_excluding) // dont do for the excluded index
            {
                float distance = DistanceToLine(line_start, line_end, points[i]);
                if (distance < distance_to_closest)
                {
                    distance_to_closest = distance;
                    index_of_closest = i;
                }
            }
        }
        return index_of_closest;
    }

    public float DistanceToLine(Vector2 line_start, Vector2 line_end, Vector2 point)
    {
        // Trig. Use magnitude of cross product. effectively doing (point - line_start).magnitude * sin(angle)
        Vector2 v1 = line_end - line_start;
        Vector2 v2 = point - line_start;
        v1.Normalize();
        return Mathf.Abs(v1.x * v2.y - v1.y * v2.x);
    }

    // ------ these functions are used to determine if two polygons are overlapping ----- //

    //poly1 and poly2 are arrays of VELatlongs that represent polygons
    bool ArePolygonsOverlapped(PolygonCollider2D poly1, PolygonCollider2D poly2)
    {
        // first do quick check to see if any points of one are contained within the other

        // check if any points of poly1 are within poly2
        for (int i = 0; i < poly1.points.Length; i++)
        {
            Vector2 world_point = poly1.transform.TransformPoint(poly1.points[i]);
            if (poly2.OverlapPoint(world_point))
            {
                return true;
            }
        }
        // check if any points of poly2 are within poly1
        for (int i = 0; i < poly2.points.Length; i++)
        {
            Vector2 world_point = poly2.transform.TransformPoint(poly2.points[i]);
            if (poly1.OverlapPoint(world_point))
            {
                return true;
            }
        }
        // now need to check if they intersect anywhere

        // close polygons
        // create new arrays to hold closed polygons and copy over data
        Vector2[] points1 = new Vector2[poly1.points.Length + 1];
        Vector2[] points2 = new Vector2[poly2.points.Length + 1];
        for (int i = 0; i < poly1.points.Length; i++)
        {
            points1[i] = poly1.transform.TransformPoint(poly1.points[i]);
        }
        points1[points1.Length - 1] = points1[0];
        for (int i = 0; i < poly2.points.Length; i++)
        {
            points2[i] = poly2.transform.TransformPoint(poly2.points[i]);
        }
        points2[points2.Length - 1] = points2[0];

        if (points1.Length >= 3 && points2.Length >= 3)
        {
            for (int i = 0; i < points1.Length - 1; i++)
            {
                for (int k = 0; k < points2.Length - 1; k++)
                {
                    if (PolylineIntersection(points1[i], points1[i + 1], points2[k], points2[k + 1]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        return false;
    }

    // returns true if the vector between points a1-a2 intersects with the vector between points b1-b2
    bool PolylineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        //Line segment 1 (a1, a2)
        float a_y_delta = a2.y - a1.y;
        float a_x_delta = a1.x - a2.x;
        float C1 = a_y_delta * a1.x + a_x_delta * a1.y;

        //Line segment 2 (b1,  b2)
        float b_y_delta = b2.y - b1.y;
        float b_x_delta = b1.x - b2.x;
        float C2 = b_y_delta * b1.x + b_x_delta * b1.y;

        // cross product magnitude (a*b*sin(angle))
        float determinate = a_y_delta * b_x_delta - b_y_delta * a_x_delta;

        if (determinate != 0)
        {
            float x = (b_x_delta * C1 - a_x_delta * C2) / determinate;
            float y = (a_y_delta * C2 - b_y_delta * C1) / determinate;

            Vector2 intersect = new Vector2(x, y);

            if (InBoundedBox(a1, a2, intersect) && InBoundedBox(b1, b2, intersect))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        { //lines are parrallel
            return false;
        }
    }

    // returns true if the test point is contained within a box defined by the diagonal corners corner1 and corner2
    bool InBoundedBox(Vector2 corner1, Vector2 corner2, Vector2 test_point)
    {
        bool between_y;
        bool between_x;

        if (corner1.y < corner2.y)
        {
            between_y = (corner1.y <= test_point.y && corner2.y >= test_point.y);
        }
        else
        {
            between_y = (corner1.y >= test_point.y && corner2.y <= test_point.y);
        }

        if (corner1.x < corner2.x)
        {
            between_x = (corner1.x <= test_point.x && corner2.x >= test_point.x);
        }
        else
        {
            between_x = (corner1.x >= test_point.x && corner2.x <= test_point.x);
        }

        return (between_y && between_x);
    }
}

