using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PNRetargetting : MonoBehaviour{
    public Transform mainModel;
    public Transform mainPNModel;
    public bool useMixamoStylePrefix = true;
    public string prefixName = "mixamorig:";
    public bool assignFingers = false;
    public bool useRootTranslation = true;
    public bool useUpDownTranslation = true;
    public bool repositionMainModelInsteadOfRoot = false;
    
    public List<Transform> MainBodyJoints;
    public Transform NeckJoint;
    public Transform HeadJoint;
    public List<Transform> RightArmJoints;
    public List<Transform> LeftArmJoints;
    public List<Transform> RightLegJoints;
    public List<Transform> LeftLegJoints;

    private List<Transform> PNMainBodyJoints = new List<Transform>();
    private Transform PNNeckJoint;
    private Transform PNHeadJoint;
    private List<Transform> PNRightArmJoints = new List<Transform>();
    private List<Transform> PNLeftArmJoints = new List<Transform>();
    private List<Transform> PNRightLegJoints = new List<Transform>();
    private List<Transform> PNLeftLegJoints = new List<Transform>();
    
    private string PNPrefixName = "Robot_";
    private string MainBodyPrefix = "Hips";
    private string RightArmPrefix = "RightShoulder";
    private string LeftArmPrefix = "LeftShoulder";
    private string RightLegPrefix = "RightUpLeg";
    private string LeftLegPrefix = "LeftUpLeg";

    private List<Quaternion> mainBodyQuaternionOffset = new List<Quaternion>();
    private Quaternion neckQuaternionOffset = new Quaternion();
    private Quaternion headQuaternionOffset = new Quaternion();
    private List<Quaternion> rightArmQuaternionOffset = new List<Quaternion>();
    private List<Quaternion> leftArmQuaternionOffset = new List<Quaternion>();
    private List<Quaternion> rightLegQuaternionOffset = new List<Quaternion>();
    private List<Quaternion> leftLegQuaternionOffset = new List<Quaternion>();

    private Vector3 mainBodyPositionOffset = new Vector3();
    private Quaternion mainBodyRotationOffset = new Quaternion();
    private Vector3 mainPNBodyPositionOffset = new Vector3();
    private float yTranslation;

    void Start()
    {
        mainBodyRotationOffset = mainModel.rotation;

        AssignPNJoints();

        if (useMixamoStylePrefix)
        {
            AssignPrefixJoints();
        }

        AssignOffsets();

        if (!repositionMainModelInsteadOfRoot)
        {
            mainBodyPositionOffset = MainBodyJoints[0].localPosition;
        }
        else
        {
            mainBodyPositionOffset = mainModel.localPosition;
        }

        mainPNBodyPositionOffset = PNMainBodyJoints[0].localPosition;
    }

    void Update()
    {
        mainBodyRotationOffset = mainModel.rotation;

        if(Input.GetKeyDown(KeyCode.A))
        {
            ResetMainBodyPositionOffset(false);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ResetMainBodyPositionOffset(true);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            useRootTranslation = !useRootTranslation;
        }
        
        ApplyPNRotationWithRotationalOffset();

        ApplyPNTranslationWithTranslationOffset();
    }

    #region Public Methods

    /// <summary>
    /// Call this before turning the PN repositioning back on (by setting useRootTranslation to true) to keep model from resnapping to starting position
    /// </summary>
    /// <param name="resetWithCurrentPNPosition"></param>
    public void ResetMainBodyPositionOffset(bool resetWithCurrentPNPosition)
    {
        if (resetWithCurrentPNPosition)
        {
            //TODO: Figure out correct subtraction so model doesn't move
            //mainBodyPositionOffset = mainModel.localPosition + (mainPNBodyPositionOffset - PNMainBodyJoints[0].localPosition);
            mainBodyPositionOffset = mainModel.localPosition - ((PNMainBodyJoints[0].localPosition - mainPNBodyPositionOffset) + mainBodyPositionOffset);
        }
        else
        {
            mainBodyPositionOffset = mainModel.localPosition;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Applies PN Translation to model with models original translation offsets.
    /// </summary>
    private void ApplyPNTranslationWithTranslationOffset()
    {
        if (!repositionMainModelInsteadOfRoot)
        {
            if (useRootTranslation)
            {
                MainBodyJoints[0].localPosition = (PNMainBodyJoints[0].localPosition - mainPNBodyPositionOffset) + mainBodyPositionOffset;
            }
            else if (useUpDownTranslation)
            {
                yTranslation = (PNMainBodyJoints[0].localPosition.y - mainPNBodyPositionOffset.y) + mainBodyPositionOffset.y;
                MainBodyJoints[0].localPosition = new Vector3(MainBodyJoints[0].localPosition.x, yTranslation, MainBodyJoints[0].localPosition.z);
            }
        }
        else
        {
            if (useRootTranslation)
            {
                // Multiplying quaternion against PN vector so that translation takes into account rotation
                mainModel.localPosition = ((mainModel.localRotation * (PNMainBodyJoints[0].localPosition - mainPNBodyPositionOffset)) + mainBodyPositionOffset);
            }
            else if (useUpDownTranslation)
            {
                yTranslation = (PNMainBodyJoints[0].localPosition.y - mainPNBodyPositionOffset.y) + mainBodyPositionOffset.y;
                mainModel.localPosition = new Vector3(mainModel.localPosition.x, yTranslation, mainModel.localPosition.z);
            }
        }
    }

    /// <summary>
    /// Applies PN Rotation to model with the models original rotational offsets.
    /// </summary>
    private void ApplyPNRotationWithRotationalOffset()
    {
        for (int i = 0; i < MainBodyJoints.Count && i < PNMainBodyJoints.Count; i++)
        {
            MainBodyJoints[i].rotation = mainBodyRotationOffset;
            MainBodyJoints[i].rotation *= PNMainBodyJoints[i].rotation;
            MainBodyJoints[i].rotation *= mainBodyQuaternionOffset[i];
        }

        NeckJoint.rotation = mainBodyRotationOffset;
        NeckJoint.rotation *= PNNeckJoint.rotation;
        NeckJoint.rotation *= neckQuaternionOffset;

        HeadJoint.rotation = mainBodyRotationOffset;
        HeadJoint.rotation *= PNHeadJoint.rotation;
        HeadJoint.rotation *= headQuaternionOffset;

        for (int i = 0; i < RightArmJoints.Count && i < PNRightArmJoints.Count; i++)
        {
            RightArmJoints[i].rotation = mainBodyRotationOffset;
            RightArmJoints[i].rotation *= PNRightArmJoints[i].rotation;
            RightArmJoints[i].rotation *= rightArmQuaternionOffset[i];
        }

        for (int i = 0; i < LeftArmJoints.Count && i < PNLeftArmJoints.Count; i++)
        {
            LeftArmJoints[i].rotation = mainBodyRotationOffset;
            LeftArmJoints[i].rotation *= PNLeftArmJoints[i].rotation;
            LeftArmJoints[i].rotation *= leftArmQuaternionOffset[i];
        }

        for (int i = 0; i < RightLegJoints.Count && i < PNRightLegJoints.Count; i++)
        {
            RightLegJoints[i].rotation = mainBodyRotationOffset;
            RightLegJoints[i].rotation *= PNRightLegJoints[i].rotation;
            RightLegJoints[i].rotation *= rightLegQuaternionOffset[i];
        }

        for (int i = 0; i < LeftLegJoints.Count && i < PNLeftLegJoints.Count; i++)
        {
            LeftLegJoints[i].rotation = mainBodyRotationOffset;
            LeftLegJoints[i].rotation *= PNLeftLegJoints[i].rotation;
            LeftLegJoints[i].rotation *= leftLegQuaternionOffset[i];
        }
    }

    /// <summary>
    /// Assign PN Robot joints
    /// </summary>
    private void AssignPNJoints()
    {
        Transform mainBody = mainPNModel.Search(PNPrefixName + MainBodyPrefix);
        Transform rightArm = mainPNModel.Search(PNPrefixName + RightArmPrefix);
        Transform leftArm = mainPNModel.Search(PNPrefixName + LeftArmPrefix);
        Transform rightLeg = mainPNModel.Search(PNPrefixName + RightLegPrefix);
        Transform leftLeg = mainPNModel.Search(PNPrefixName + LeftLegPrefix);

        PNMainBodyJoints = new List<Transform>();
        PNMainBodyJoints.Add(mainBody);
        PNMainBodyJoints.Add(mainBody.Find(PNPrefixName + "Spine"));
        PNMainBodyJoints.Add(PNMainBodyJoints[1].Find(PNPrefixName + "Spine1"));
        PNMainBodyJoints.Add(PNMainBodyJoints[2].Find(PNPrefixName + "Spine2"));
        PNMainBodyJoints.Add(PNMainBodyJoints[3].Find(PNPrefixName + "Spine3"));

        PNNeckJoint = mainPNModel.Search(PNPrefixName + "Neck");
        PNHeadJoint = mainPNModel.Search(PNPrefixName + "Head");

        rightArm.GetComponentsInChildren<Transform>(PNRightArmJoints);
        leftArm.GetComponentsInChildren<Transform>(PNLeftArmJoints);
        rightLeg.GetComponentsInChildren<Transform>(PNRightLegJoints);
        leftLeg.GetComponentsInChildren<Transform>(PNLeftLegJoints);
    }

    /// <summary>
    /// Assign user model joints based on Mixamo joint naming convention
    /// </summary>
    private void AssignPrefixJoints()
    {
        Transform mainBody = mainModel.Search(prefixName + MainBodyPrefix).transform;
        Transform rightArm = mainModel.Search(prefixName + RightArmPrefix).transform;
        Transform leftArm = mainModel.Search(prefixName + LeftArmPrefix).transform;
        Transform rightLeg = mainModel.Search(prefixName + RightLegPrefix).transform;
        Transform leftLeg = mainModel.Search(prefixName + LeftLegPrefix).transform;

        MainBodyJoints = new List<Transform>();
        MainBodyJoints.Add(mainBody);
        MainBodyJoints.Add(mainBody.Find(prefixName + "Spine"));
        MainBodyJoints.Add(MainBodyJoints[1].Find(prefixName + "Spine1"));
        MainBodyJoints.Add(MainBodyJoints[2].Find(prefixName + "Spine2"));

        NeckJoint = mainModel.Search(prefixName + "Neck").transform;
        HeadJoint = mainModel.Search(prefixName + "Head").transform;

        if (assignFingers)
        {
            rightArm.GetComponentsInChildren<Transform>(RightArmJoints);
            leftArm.GetComponentsInChildren<Transform>(LeftArmJoints);
        }
        else
        {
            RightArmJoints = new List<Transform>();
            LeftArmJoints = new List<Transform>();

            RightArmJoints.Add(rightArm);
            RightArmJoints.Add(rightArm.Find(prefixName + "RightArm"));
            RightArmJoints.Add(RightArmJoints[1].Find(prefixName + "RightForeArm"));
            RightArmJoints.Add(RightArmJoints[2].Find(prefixName + "RightHand"));

            RightArmJoints.Add(RightArmJoints[3].Find(prefixName + "RightHandThumb1"));
            RightArmJoints.Add(RightArmJoints[4].Find(prefixName + "RightHandThumb2"));
            RightArmJoints.Add(RightArmJoints[5].Find(prefixName + "RightHandThumb3"));
            RightArmJoints.Add(RightArmJoints[6].Find(prefixName + "RightHandThumb4"));

            RightArmJoints.Add(RightArmJoints[3].Find(prefixName + "RightHandIndex1"));
            RightArmJoints.Add(RightArmJoints[3].Find(prefixName + "RightHandIndex1"));
            RightArmJoints.Add(RightArmJoints[9].Find(prefixName + "RightHandIndex2"));
            RightArmJoints.Add(RightArmJoints[10].Find(prefixName + "RightHandIndex3"));
            RightArmJoints.Add(RightArmJoints[11].Find(prefixName + "RightHandIndex4"));

            RightArmJoints.Add(RightArmJoints[3].Find(prefixName + "RightHandMiddle1"));
            RightArmJoints.Add(RightArmJoints[3].Find(prefixName + "RightHandMiddle1"));
            RightArmJoints.Add(RightArmJoints[14].Find(prefixName + "RightHandMiddle2"));
            RightArmJoints.Add(RightArmJoints[15].Find(prefixName + "RightHandMiddle3"));
            RightArmJoints.Add(RightArmJoints[16].Find(prefixName + "RightHandMiddle4"));

            RightArmJoints.Add(RightArmJoints[3].Find(prefixName + "RightHandPinky1"));
            RightArmJoints.Add(RightArmJoints[3].Find(prefixName + "RightHandPinky1"));
            RightArmJoints.Add(RightArmJoints[19].Find(prefixName + "RightHandPinky2"));
            RightArmJoints.Add(RightArmJoints[20].Find(prefixName + "RightHandPinky3"));
            RightArmJoints.Add(RightArmJoints[21].Find(prefixName + "RightHandPinky4"));

            RightArmJoints.Add(RightArmJoints[3].Find(prefixName + "RightHandRing1"));
            RightArmJoints.Add(RightArmJoints[3].Find(prefixName + "RightHandRing1"));
            RightArmJoints.Add(RightArmJoints[24].Find(prefixName + "RightHandRing2"));
            RightArmJoints.Add(RightArmJoints[25].Find(prefixName + "RightHandRing3"));
            RightArmJoints.Add(RightArmJoints[26].Find(prefixName + "RightHandRing4"));

            LeftArmJoints.Add(leftArm);
            LeftArmJoints.Add(leftArm.Find(prefixName + "LeftArm"));
            LeftArmJoints.Add(LeftArmJoints[1].Find(prefixName + "LeftForeArm"));
            LeftArmJoints.Add(LeftArmJoints[2].Find(prefixName + "LeftHand"));

            LeftArmJoints.Add(LeftArmJoints[3].Find(prefixName + "LeftHandThumb1"));
            LeftArmJoints.Add(LeftArmJoints[4].Find(prefixName + "LeftHandThumb2"));
            LeftArmJoints.Add(LeftArmJoints[5].Find(prefixName + "LeftHandThumb3"));
            LeftArmJoints.Add(LeftArmJoints[6].Find(prefixName + "LeftHandThumb4"));

            LeftArmJoints.Add(LeftArmJoints[3].Find(prefixName + "LeftHandIndex1"));
            LeftArmJoints.Add(LeftArmJoints[3].Find(prefixName + "LeftHandIndex1"));
            LeftArmJoints.Add(LeftArmJoints[9].Find(prefixName + "LeftHandIndex2"));
            LeftArmJoints.Add(LeftArmJoints[10].Find(prefixName + "LeftHandIndex3"));
            LeftArmJoints.Add(LeftArmJoints[11].Find(prefixName + "LeftHandIndex4"));

            LeftArmJoints.Add(LeftArmJoints[3].Find(prefixName + "LeftHandMiddle1"));
            LeftArmJoints.Add(LeftArmJoints[3].Find(prefixName + "LeftHandMiddle1"));
            LeftArmJoints.Add(LeftArmJoints[14].Find(prefixName + "LeftHandMiddle2"));
            LeftArmJoints.Add(LeftArmJoints[15].Find(prefixName + "LeftHandMiddle3"));
            LeftArmJoints.Add(LeftArmJoints[16].Find(prefixName + "LeftHandMiddle4"));

            LeftArmJoints.Add(LeftArmJoints[3].Find(prefixName + "LeftHandPinky1"));
            LeftArmJoints.Add(LeftArmJoints[3].Find(prefixName + "LeftHandPinky1"));
            LeftArmJoints.Add(LeftArmJoints[19].Find(prefixName + "LeftHandPinky2"));
            LeftArmJoints.Add(LeftArmJoints[20].Find(prefixName + "LeftHandPinky3"));
            LeftArmJoints.Add(LeftArmJoints[21].Find(prefixName + "LeftHandPinky4"));

            LeftArmJoints.Add(LeftArmJoints[3].Find(prefixName + "LeftHandRing1"));
            LeftArmJoints.Add(LeftArmJoints[3].Find(prefixName + "LeftHandRing1"));
            LeftArmJoints.Add(LeftArmJoints[24].Find(prefixName + "LeftHandRing2"));
            LeftArmJoints.Add(LeftArmJoints[25].Find(prefixName + "LeftHandRing3"));
            LeftArmJoints.Add(LeftArmJoints[26].Find(prefixName + "LeftHandRing4"));
        }

        rightLeg.GetComponentsInChildren<Transform>(RightLegJoints);
        leftLeg.GetComponentsInChildren<Transform>(LeftLegJoints);
    }

    /// <summary>
    /// Assign rotational offsets for each joint so that we know how much to rotate each joint 
    /// when zero'd out to match PN rig.
    /// </summary>
    private void AssignOffsets()
    {
        for (int i = 0; i < MainBodyJoints.Count; i++)
        {
            mainBodyQuaternionOffset.Add(Quaternion.Inverse(mainBodyRotationOffset) * MainBodyJoints[i].rotation);
        }

        neckQuaternionOffset = Quaternion.Inverse(mainBodyRotationOffset) * NeckJoint.rotation;
        headQuaternionOffset = Quaternion.Inverse(mainBodyRotationOffset) * HeadJoint.rotation;

        for (int i = 0; i < RightArmJoints.Count; i++)
        {
            rightArmQuaternionOffset.Add(Quaternion.Inverse(mainBodyRotationOffset) * RightArmJoints[i].rotation);
        }

        for (int i = 0; i < LeftArmJoints.Count; i++)
        {
            leftArmQuaternionOffset.Add(Quaternion.Inverse(mainBodyRotationOffset) * LeftArmJoints[i].rotation);
        }

        for (int i = 0; i < RightLegJoints.Count; i++)
        {
            rightLegQuaternionOffset.Add(Quaternion.Inverse(mainBodyRotationOffset) * RightLegJoints[i].rotation);
        }

        for (int i = 0; i < LeftLegJoints.Count; i++)
        {
            leftLegQuaternionOffset.Add(Quaternion.Inverse(mainBodyRotationOffset) * LeftLegJoints[i].rotation);
        }
    }

    #endregion

}

public static class Extensions{
    public static Transform Search(this Transform target, string name)
    {
        if (target.name == name) return target;

        for (int i = 0; i < target.childCount; ++i)
        {
            var result = Search(target.GetChild(i), name);

            if (result != null) return result;
        }

        return null;
    }
}
