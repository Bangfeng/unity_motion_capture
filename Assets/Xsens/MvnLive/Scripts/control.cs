using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using xsens;
public class control : XsLiveAnimator
{
    public XsStreamReader mvnActors;            //network streamer, which contains all 4 actors' poses
    public int actorID = 1;                     //current actor ID, where 1 is the first streamed character from MVN

    public bool applyRootMotion = true;         //if true, position will be applied to the root (pelvis)

    public XsProp prop1;
    public XsProp prop2;
    public XsProp prop3;
    public XsProp prop4;
    private XsProp[] props;
    private XsPropManager[] targetProps;
    private GameObject[] currentProps;
    private GameObject obj1;
    private GameObject obj2;
    private Transform mvnActor;                 //reference for MVN Actor. This has the same layout as the streamed pose.
    private Transform target;                   //Reference to the character in Unity3D.
    private Transform origPos;                  //original position of the animation, this is the zero
    private Transform[] targetModel;            //Model holds each segments
    private Transform[] currentPose;            //animation applyed on skeleton. global position and rotation, used to represent a pose	
    private Quaternion[] modelRotTP;            //T-Pose reference rotation on each segment
    private Vector3[] modelPosTP;               //T-Pose reference position on each segment
    private GameObject missingSegments;         //Empty object for not used segments
    private Animator animator;                  //Animator object to get the Humanoid character mapping correct.
    private Dictionary<XsBodyAnimationSegment, HumanBodyBones> bodyMecanimBones;
    private Dictionary<XsFingerAnimationSegment, HumanBodyBones> fingerMechanimBones;
    private bool isInited;                      //flag to check if the plugin was correctly intialized.
    private int segmentCount = 0;               //used to figure out the total segment count provided by the data
    private bool fingerTrackingEnabled;         //toggles setting up the finger transforms 
    private int propDataSize = 0;                  //used to offset the index of incoming data since props sit between body segments and finger segments

#if TPOSE_FIRST
        private bool isFirstPose;					//check if the first pose is passed
#endif
    private bool isDebugFrame = false;          //debug animation skeleton
                                                /// </summary>
    public enum XsBodyAnimationSegment
    {
        Pelvis = 0,

        L5 = 1,//not used
        L3 = 2,//spine
        T12 = 3,//not used
        T8 = 4,//chest

        Neck = 5,
        Head = 6,

        RightShoulder = 7,
        RightUpperArm = 8,
        RightLowerArm = 9,
        RightHand = 10,

        LeftShoulder = 11,
        LeftUpperArm = 12,
        LeftLowerArm = 13,
        LeftHand = 14,

        RightUpperLeg = 15,
        RightLowerLeg = 16,
        RightFoot = 17,
        RightToe = 18,

        LeftUpperLeg = 19,
        LeftLowerLeg = 20,
        LeftFoot = 21,
        LeftToe = 22
    }
    int[] bodySegmentOrder =
       {
                    (int)XsBodyAnimationSegment.Pelvis,

                    (int)XsBodyAnimationSegment.L5,
                    (int)XsBodyAnimationSegment.L3,
                    (int)XsBodyAnimationSegment.T12,
                    (int)XsBodyAnimationSegment.T8,

                    (int)XsBodyAnimationSegment.Neck,
                    (int)XsBodyAnimationSegment.Head,

                    (int)XsBodyAnimationSegment.RightShoulder,
                    (int)XsBodyAnimationSegment.RightUpperArm,
                    (int)XsBodyAnimationSegment.RightLowerArm,
                    (int)XsBodyAnimationSegment.RightHand,

                    (int)XsBodyAnimationSegment.LeftShoulder,
                    (int)XsBodyAnimationSegment.LeftUpperArm,
                    (int)XsBodyAnimationSegment.LeftLowerArm,
                    (int)XsBodyAnimationSegment.LeftHand,

                    (int)XsBodyAnimationSegment.RightUpperLeg,
                    (int)XsBodyAnimationSegment.RightLowerLeg,
                    (int)XsBodyAnimationSegment.RightFoot,
                    (int)XsBodyAnimationSegment.RightToe,

                    (int)XsBodyAnimationSegment.LeftUpperLeg,
                    (int)XsBodyAnimationSegment.LeftLowerLeg,
                    (int)XsBodyAnimationSegment.LeftFoot,
                    (int)XsBodyAnimationSegment.LeftToe
        };
    protected void mapMecanimBones()
    {
        bodyMecanimBones = new Dictionary<XsBodyAnimationSegment, HumanBodyBones>();

        bodyMecanimBones.Add(XsBodyAnimationSegment.Pelvis, HumanBodyBones.Hips);
        bodyMecanimBones.Add(XsBodyAnimationSegment.LeftUpperLeg, HumanBodyBones.LeftUpperLeg);
        bodyMecanimBones.Add(XsBodyAnimationSegment.LeftLowerLeg, HumanBodyBones.LeftLowerLeg);
        bodyMecanimBones.Add(XsBodyAnimationSegment.LeftFoot, HumanBodyBones.LeftFoot);
        bodyMecanimBones.Add(XsBodyAnimationSegment.LeftToe, HumanBodyBones.LeftToes);
        bodyMecanimBones.Add(XsBodyAnimationSegment.RightUpperLeg, HumanBodyBones.RightUpperLeg);
        bodyMecanimBones.Add(XsBodyAnimationSegment.RightLowerLeg, HumanBodyBones.RightLowerLeg);
        bodyMecanimBones.Add(XsBodyAnimationSegment.RightFoot, HumanBodyBones.RightFoot);
        bodyMecanimBones.Add(XsBodyAnimationSegment.RightToe, HumanBodyBones.RightToes);
        bodyMecanimBones.Add(XsBodyAnimationSegment.L5, HumanBodyBones.LastBone);   //not used
        bodyMecanimBones.Add(XsBodyAnimationSegment.L3, HumanBodyBones.Spine);
        bodyMecanimBones.Add(XsBodyAnimationSegment.T12, HumanBodyBones.LastBone);  //not used
        bodyMecanimBones.Add(XsBodyAnimationSegment.T8, HumanBodyBones.Chest);
        bodyMecanimBones.Add(XsBodyAnimationSegment.LeftShoulder, HumanBodyBones.LeftShoulder);
        bodyMecanimBones.Add(XsBodyAnimationSegment.LeftUpperArm, HumanBodyBones.LeftUpperArm);
        bodyMecanimBones.Add(XsBodyAnimationSegment.LeftLowerArm, HumanBodyBones.LeftLowerArm);
        bodyMecanimBones.Add(XsBodyAnimationSegment.LeftHand, HumanBodyBones.LeftHand);
        bodyMecanimBones.Add(XsBodyAnimationSegment.RightShoulder, HumanBodyBones.RightShoulder);
        bodyMecanimBones.Add(XsBodyAnimationSegment.RightUpperArm, HumanBodyBones.RightUpperArm);
        bodyMecanimBones.Add(XsBodyAnimationSegment.RightLowerArm, HumanBodyBones.RightLowerArm);
        bodyMecanimBones.Add(XsBodyAnimationSegment.RightHand, HumanBodyBones.RightHand);
        bodyMecanimBones.Add(XsBodyAnimationSegment.Neck, HumanBodyBones.Neck);
        bodyMecanimBones.Add(XsBodyAnimationSegment.Head, HumanBodyBones.Head);
    }
    public bool setupMvnActor()
    {
        mvnActor.rotation = transform.rotation;
        mvnActor.position = transform.position;

        currentPose[(int)XsBodyAnimationSegment.Pelvis] = mvnActor.Find("Pelvis");
        currentPose[(int)XsBodyAnimationSegment.L5] = mvnActor.Find("Pelvis/L5");

        currentPose[(int)XsBodyAnimationSegment.L3] = mvnActor.Find("Pelvis/L5/L3");
        currentPose[(int)XsBodyAnimationSegment.T12] = mvnActor.Find("Pelvis/L5/L3/T12");
        currentPose[(int)XsBodyAnimationSegment.T8] = mvnActor.Find("Pelvis/L5/L3/T12/T8");
        currentPose[(int)XsBodyAnimationSegment.LeftShoulder] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder");
        currentPose[(int)XsBodyAnimationSegment.LeftUpperArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder/LeftUpperArm");
        currentPose[(int)XsBodyAnimationSegment.LeftLowerArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder/LeftUpperArm/LeftLowerArm");
        currentPose[(int)XsBodyAnimationSegment.LeftHand] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder/LeftUpperArm/LeftLowerArm/LeftHand");

        currentPose[(int)XsBodyAnimationSegment.Neck] = mvnActor.Find("Pelvis/L5/L3/T12/T8/Neck");
        currentPose[(int)XsBodyAnimationSegment.Head] = mvnActor.Find("Pelvis/L5/L3/T12/T8/Neck/Head");

        currentPose[(int)XsBodyAnimationSegment.RightShoulder] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder");
        currentPose[(int)XsBodyAnimationSegment.RightUpperArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder/RightUpperArm");
        currentPose[(int)XsBodyAnimationSegment.RightLowerArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder/RightUpperArm/RightLowerArm");
        currentPose[(int)XsBodyAnimationSegment.RightHand] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder/RightUpperArm/RightLowerArm/RightHand");

        currentPose[(int)XsBodyAnimationSegment.LeftUpperLeg] = mvnActor.Find("Pelvis/LeftUpperLeg");
        currentPose[(int)XsBodyAnimationSegment.LeftLowerLeg] = mvnActor.Find("Pelvis/LeftUpperLeg/LeftLowerLeg");
        currentPose[(int)XsBodyAnimationSegment.LeftFoot] = mvnActor.Find("Pelvis/LeftUpperLeg/LeftLowerLeg/LeftFoot");
        currentPose[(int)XsBodyAnimationSegment.LeftToe] = mvnActor.Find("Pelvis/LeftUpperLeg/LeftLowerLeg/LeftFoot/LeftToe");
        currentPose[(int)XsBodyAnimationSegment.RightUpperLeg] = mvnActor.Find("Pelvis/RightUpperLeg");
        currentPose[(int)XsBodyAnimationSegment.RightLowerLeg] = mvnActor.Find("Pelvis/RightUpperLeg/RightLowerLeg");
        currentPose[(int)XsBodyAnimationSegment.RightFoot] = mvnActor.Find("Pelvis/RightUpperLeg/RightLowerLeg/RightFoot");
        currentPose[(int)XsBodyAnimationSegment.RightToe] = mvnActor.Find("Pelvis/RightUpperLeg/RightLowerLeg/RightFoot/RightToe");

        return true;
    }
    public bool setupModel(Transform model, Transform[] modelRef)
    {
        animator = model.GetComponent<Animator>();
        if (!animator)
        {
            return false;
        }

        //face the input model same as our animation
        model.rotation = transform.rotation;
        model.position = transform.position;

        //go through the model's body segments and store values
        for (int i = 0; i < XsMvnPose.MvnBodySegmentCount; i++)
        {

            XsBodyAnimationSegment segID = (XsBodyAnimationSegment)bodySegmentOrder[i];
            HumanBodyBones boneID = bodyMecanimBones[(XsBodyAnimationSegment)bodySegmentOrder[i]];
            try
            {

                if (boneID == HumanBodyBones.LastBone)
                {
                    //not used bones
                    modelRef[(int)segID] = null;
                    modelPosTP[(int)segID] = Vector3.zero;
                    modelRotTP[(int)segID] = Quaternion.Euler(Vector3.zero);

                }
                else
                {
                    //used bones
                    Transform tmpTransf = animator.GetBoneTransform(boneID);
                    Vector3 tempPos = transform.position;
                    Quaternion tempRot = transform.rotation;

                    transform.position = Vector3.zero;
                    transform.rotation = Quaternion.identity;

                    modelRef[(int)segID] = tmpTransf;
                    modelPosTP[(int)segID] = modelRef[(int)segID].position;
                    modelRotTP[(int)segID] = modelRef[(int)segID].rotation;

                    transform.position = tempPos;
                    transform.rotation = tempRot;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[xsens] Can't find " + boneID + " in the model! " + e);
                modelRef[(int)segID] = null;
                modelPosTP[(int)segID] = Vector3.zero;
                modelRotTP[(int)segID] = Quaternion.Euler(Vector3.zero);

                return false;
            }

        }

        //set our starting index
        int startingPoiont = XsMvnPose.MvnBodySegmentCount;
        //iterate through the prop segment count and setup each prop
        for (int i = 0; i < XsMvnPose.MvnPropSegmentCount; i++)
        {
            try
            {
                if (props[i] == null)
                {
                    modelRef[startingPoiont + i] = null;
                    modelPosTP[startingPoiont + i] = Vector3.zero;
                    modelRotTP[startingPoiont + i] = Quaternion.Euler(Vector3.zero);
                }
                else
                {
                    GameObject prop = Instantiate(props[i].SpawnProp());
                    targetProps[i] = prop.GetComponent<XsPropManager>();
                    prop.transform.parent = targetModel[(int)props[i].segment];

                    modelRef[startingPoiont + i] = prop.transform;
                    modelPosTP[startingPoiont + i] = modelRef[startingPoiont + i].position;
                    modelRotTP[startingPoiont + i] = modelRef[startingPoiont + i].rotation;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[xsens] Can't find prop " + i + ". Pleast make sure you add it to the XsLiveAnimator " + e);
                modelRef[startingPoiont + i] = null;
                modelPosTP[startingPoiont + i] = Vector3.zero;
                modelRotTP[startingPoiont + i] = Quaternion.Euler(Vector3.zero);

                return false;
            }
        }

        //go through the model's finger segments and store values

        return true;
    }
    void TposeModel()
    {
        for (int i = 0; i < modelRotTP.Length; i++)
        {
            if (targetModel[i] != null)
            {
                targetModel[i].position = modelPosTP[i];
                targetModel[i].rotation = modelRotTP[i];
            }
        }
    }
    protected void initSkeleton(Transform[] model, Vector3[] positions, Quaternion[] orientations)
    {

        //wait for real data
        if (positions[(int)XsBodyAnimationSegment.Pelvis] == Vector3.zero)
        {
            return;
        }

        //reposition the segments based on the data
#if TPOSE_FIRST
            isFirstPose = false;
#endif

        for (int i = 0; i < bodySegmentOrder.Length; i++)
        {
            if (XsBodyAnimationSegment.Pelvis == (XsBodyAnimationSegment)bodySegmentOrder[i])
            {
                //global for pelvis
                model[bodySegmentOrder[i]].transform.position = positions[bodySegmentOrder[i]];
                model[bodySegmentOrder[i]].transform.rotation = orientations[bodySegmentOrder[i]];

            }
            else
            {
                //local for segments
                model[bodySegmentOrder[i]].transform.localPosition = positions[bodySegmentOrder[i]];
                model[bodySegmentOrder[i]].transform.localRotation = orientations[bodySegmentOrder[i]];
            }
        }

        //reinit the actor
        setupMvnActor();
    }
    private void updateMvnActor(Transform[] model, Vector3[] positions, Quaternion[] orientations)
    {
        try
        {
            for (int i = 0; i < bodySegmentOrder.Length; i++)   //front
            {
                if (XsBodyAnimationSegment.Pelvis == (XsBodyAnimationSegment)bodySegmentOrder[i])
                {
                    //we apply global position and orientaion to the pelvis
                    if (applyRootMotion)
                    {
                        model[bodySegmentOrder[i]].transform.position = (positions[bodySegmentOrder[i]] + origPos.position) * transform.localScale.x;
                    }
                    else
                    {
                        model[bodySegmentOrder[i]].transform.localPosition =
                            new Vector3(model[bodySegmentOrder[i]].transform.position.x,
                            positions[bodySegmentOrder[i]].y + origPos.position.y,
                            model[bodySegmentOrder[i]].transform.position.z);
                    }
                    Quaternion orientation =
                        Quaternion.Inverse(model[i].transform.parent.rotation)
                         * orientations[bodySegmentOrder[i]]
                        * modelRotTP[i];

                    model[bodySegmentOrder[i]].transform.localRotation = orientation;

                }
                else
                {
                    if (model[bodySegmentOrder[i]] == null)
                    {
                        Debug.LogError("[xsens] XsLiveAnimator: Missing bone from mvn actor! Did you change MvnLive plugin? Please check if you are using the right actor!");
                        break;
                    }
                    Quaternion orientation =
                        Quaternion.Inverse(model[i].transform.parent.rotation)
                         * orientations[bodySegmentOrder[i]]
                        * modelRotTP[i];

                    model[bodySegmentOrder[i]].transform.localRotation = orientation;

                    //draw wireframe for original animation
                    if (isDebugFrame)
                    {

                        Color color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                        int id = bodySegmentOrder[i];
                        if (model[id - 1] != null)
                        {
                            if (((id - 1) != (int)XsBodyAnimationSegment.LeftHand)
                            && ((id - 1) != (int)XsBodyAnimationSegment.RightHand)
                            && ((id - 1) != (int)XsBodyAnimationSegment.Head)
                            && ((id - 1) != (int)XsBodyAnimationSegment.LeftToe)
                            && ((id - 1) != (int)XsBodyAnimationSegment.RightToe))
                            {
                                Debug.DrawLine(model[id].position, model[id - 1].position, color);
                            }
                        }
                    }//isDebugFrame
                }

            }//for i

            //if we have props to animate
            if (propDataSize > 0 && props.Length != 0)
            {
                int startingIndex = XsMvnPose.MvnBodySegmentCount;
                for (int i = 0; i < propDataSize; i++)
                {
                    if (model[startingIndex + i] == null)
                    {
                        Debug.LogError("[xsens] XsLiveAnimator: Missing prop from mvn actor! Did you change MvnLive plugin? Please check if you are using the right actor!");
                        break;
                    }
                    else
                    {
                        model[startingIndex + i].transform.position = model[startingIndex + i].transform.parent.position + (positions[startingIndex + i] - positions[(int)props[i].segment]) * transform.localScale.x;

                        Quaternion orientation =
                            Quaternion.Inverse(model[startingIndex + i].transform.parent.rotation)
                             * orientations[startingIndex + i]
                            * modelRotTP[startingIndex + i];
                        model[startingIndex + i].transform.localRotation = orientation;
                    }
                }//for i
            }//if props

            //if we have finger data

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    private void updateModel(Transform[] pose, Transform[] model)
    {
        //reset the target, then set it based on segments
        Vector3 pelvisPos = new Vector3();
        Vector3 lastPos = target.position;

        // if(applyRootMotion) target.position = Vector3.zero;
        for (int i = 0; i < XsMvnPose.MvnBodySegmentCount; i++)
        {
            switch (i)
            {
                //no update required
                case (int)XsBodyAnimationSegment.L5:
                case (int)XsBodyAnimationSegment.T12:
                    break;

                case (int)XsBodyAnimationSegment.Pelvis:
                    //position only on the y axis, leave the x,z to the body
                    pelvisPos = (pose[i].localPosition);
                    model[i].localPosition = new Vector3(0, pelvisPos.y, 0);

                    model[i].rotation = pose[i].rotation;
                    break;

                default:
                    //only update rotation for rest of the segments
                    if (model[i] != null)
                    {
                        model[i].rotation = pose[i].rotation;
                    }
                    break;
            }
        }

        if (propDataSize > 0 && props.Length != 0)
        {
            int startingIndex = XsMvnPose.MvnBodySegmentCount;
            for (int i = 0; i < propDataSize; i++)
            {
                Vector3 propPos = (pose[i + startingIndex].localPosition);
                model[i + startingIndex].localPosition = propPos;
                if (model[i + startingIndex] != null && pose[i + startingIndex] != null)
                {
                    model[i + startingIndex].rotation = pose[i + startingIndex].rotation;
                }
            }//for i
        }//if props



        //apply root motion if flag enabled only
        if (applyRootMotion)
        {
            model[0].transform.parent.transform.localPosition = new Vector3(pelvisPos.x, 0, pelvisPos.z);
        }
        else
        {
            model[0].transform.parent.transform.localPosition = Vector3.zero;
        }
    }
    //void Awake()
    //{

    //    try
    //    {
    //        //map each bone with xsens bipad model and mecanim bones
    //        mapMecanimBones();



    //        //setup arrays for pose's
    //        targetModel = new Transform[XsMvnPose.MvnBodySegmentCount + XsMvnPose.MvnFingerSegmentCount + XsMvnPose.MvnPropSegmentCount];
    //        modelRotTP = new Quaternion[XsMvnPose.MvnBodySegmentCount + XsMvnPose.MvnFingerSegmentCount + XsMvnPose.MvnPropSegmentCount];
    //        modelPosTP = new Vector3[XsMvnPose.MvnBodySegmentCount + XsMvnPose.MvnFingerSegmentCount + XsMvnPose.MvnPropSegmentCount];
    //        currentPose = new Transform[XsMvnPose.MvnBodySegmentCount + XsMvnPose.MvnFingerSegmentCount + XsMvnPose.MvnPropSegmentCount];

    //        //add an empty object, which we can use for missing segments
    //        missingSegments = new GameObject("MissingSegments");
    //        missingSegments.transform.parent = gameObject.transform;


    //        //setup the animation and the model as well
    //        if (!setupMvnActor())
    //        {
    //            Debug.Log("[xsens] failed to init MvnActor");
    //            return;
    //        }

    //        if (!setupModel(target, targetModel))
    //        {
    //            return;
    //        }

    //        //face model to the right direction	
    //        target.transform.rotation = transform.rotation;

    //        isInited = true;

    //        //setupMvnActor();
    //    }
    //    catch (Exception e)
    //    {
    //        print("[xsens] Something went wrong setting up.");
    //        Debug.LogException(e);
    //    }
    //}
    IEnumerator Start()
    {
        isInited = false;
#if TPOSE_FIRST
            isFirstPose = true;
#endif
        //save start positions
        target = gameObject.transform;
        origPos = target;

        //create an MvnActor 
        GameObject obj = (GameObject)Instantiate(Resources.Load("MvnActor"));
        obj.transform.parent = gameObject.transform;
        mvnActor = obj.transform;
        if (mvnActor == null)
        {
            Debug.LogError("[xsens] No AnimationSkeleton found!");
            yield return null;
        }

        // Search for the network stream, so we can communicate with it.
        if (mvnActors == null)
        {
            Debug.LogError("[xsens] No MvnActor found! You must assign an MvnActor to this component.");
            yield return null;
        }

        //Wait for data to come in so that we can figure out incomming segment counts before setup 
        while (!mvnActors.poseEstablished(actorID, out segmentCount))
        {
            yield return null;
        }




        beginSetup();

    }
    private void beginSetup()
    {
        //Work out way through the segment count cominations to figure out what we need to setup


        try
        {
            //map each bone with xsens bipad model and mecanim bones
            mapMecanimBones();
            obj1 = GameObject.FindWithTag("LEFThip");
            obj2 = GameObject.Find("Cube");

            //setup arrays for pose's
            targetModel = new Transform[XsMvnPose.MvnBodySegmentCount + XsMvnPose.MvnFingerSegmentCount + XsMvnPose.MvnPropSegmentCount];
            modelRotTP = new Quaternion[XsMvnPose.MvnBodySegmentCount + XsMvnPose.MvnFingerSegmentCount + XsMvnPose.MvnPropSegmentCount];
            modelPosTP = new Vector3[XsMvnPose.MvnBodySegmentCount + XsMvnPose.MvnFingerSegmentCount + XsMvnPose.MvnPropSegmentCount];
            currentPose = new Transform[XsMvnPose.MvnBodySegmentCount + XsMvnPose.MvnFingerSegmentCount + XsMvnPose.MvnPropSegmentCount];

            //add an empty object, which we can use for missing segments
            missingSegments = new GameObject("MissingSegments");
            missingSegments.transform.parent = gameObject.transform;


            //setup the animation and the model as well
            if (!setupMvnActor())
            {
                Debug.Log("[xsens] failed to init MvnActor");
                return;
            }

            if (!setupModel(target, targetModel))
            {
                return;
            }

            //face model to the right direction	
            target.transform.rotation = transform.rotation;

            isInited = true;

           
        }
        catch (Exception e)
        {
            print("[xsens] Something went wrong setting up.");
            Debug.LogException(e);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {


            //transform.Rotate(0, 5, 0);
            //transform.Find("Head").Rotate(0, 5, 3);
            //transform.Find("Hips").Rotate(0, 5, 3);
            //transform.Find("Hips/LeftHip").Rotate(0, 0, 3);
            transform.Find("Hips/LeftHip/LeftKnee").Rotate(0, 0, -3); 
            //obj1.transform.Rotate(0, 5, 3);
            //obj2.transform.Rotate(0, 5, 3);
            //mvnActor.Rotate(new Vector3(0, 5, 0));
            //Quaternion orientation;

        }
    }
}
//void Update () {
//        if (Input.GetKeyDown(KeyCode.W))
//        {
            
//            transform.Rotate(new Vector3(0, 5, 0));
//            Quaternion orientation;
//            //print(Convert.ToString(currentPose[bodySegmentOrder[15]]));
//            //FindNameAllChild(mvnActor, "Pelvis/LeftUpperLeg").transform.rotation = Quaternion.Euler(33, 33, 33);
//            //mvnActor.Find("Pelvis/LeftUpperLeg").transform.rotation = Quaternion.Euler(33, 33, 33);
//            //currentPose[bodySegmentOrder[15]].transform.Rotate(new Vector3(0, 5, 0));
//        }
//    }

