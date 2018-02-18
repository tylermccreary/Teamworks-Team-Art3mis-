using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {

    internal Rigidbody RBody;
    public GameObject ropePart;
    public int length = 5;

    internal void Awake()
    {
        this.gameObject.AddComponent<Rigidbody>();
        this.RBody = this.gameObject.GetComponent<Rigidbody>();
        this.RBody.isKinematic = true;

        int childCount = this.transform.childCount;
        Debug.Log("Child Count for rope is: " + childCount);

        for (int i = 0; i < childCount; i++)
        {
            Transform t = this.transform.GetChild(i);

            t.gameObject.AddComponent<HingeJoint>();
            //t.gameObject.AddComponent<Rigidbody>();

            HingeJoint hinge = t.gameObject.GetComponent<HingeJoint>();

            hinge.connectedBody =
                i == 0 ? this.RBody : this.transform.GetChild(i - 1).GetComponent<Rigidbody>();
            hinge.useSpring = true;
            hinge.enableCollision = true;
        }
    }
    
    /* private void Start()
    {
        // on Awake, create rope parts equal to length and parent them to the Rope
        for (int i = 0; i < length; i++)
        {
            //GameObject rPart = new GameObject("RopePart(" + i + ")");
            GameObject rPart = Instantiate(ropePart);
            rPart.transform.SetParent(this.transform);
            rPart.transform.localPosition = new Vector3(0, -i, 0);
        }

        StartCoroutine(AddHingeToRopeParts());
    }*/

   /* IEnumerator AddHingeToRopeParts()
    {

        yield return new WaitForSeconds(5);
        this.gameObject.AddComponent<Rigidbody>();
        this.RBody = this.gameObject.GetComponent<Rigidbody>();
        this.RBody.isKinematic = true;

        int childCount = this.transform.childCount;
        Debug.Log("Child Count for rope is: " + childCount);

        for(int i = 0; i < childCount; i++)
        {
            Transform t = this.transform.GetChild(i);

            t.gameObject.AddComponent<HingeJoint>();
            //t.gameObject.AddComponent<Rigidbody>();

            HingeJoint hinge = t.gameObject.GetComponent<HingeJoint>();

            hinge.connectedBody =
                i == 0 ? this.RBody : this.transform.GetChild(i - 1).GetComponent<Rigidbody>();
            hinge.useSpring = true;
            hinge.enableCollision = true;
        }
    }*/

}
