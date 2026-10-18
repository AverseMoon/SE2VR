using System.Collections.Immutable;
using Keen.VRage.Animation.Client.Resources;
using Keen.VRage.Core;
using Keen.VRage.Game.Components.Models;
using Keen.VRage.Library.Definitions;
using Keen.VRage.Library.Serialization;
using Keen.VRage.Library.Units;
using Keen.VRage.Library.Utils;

namespace SE2VR.Client.Configurations;

/// <summary>
/// Configuration for animating the hands/arms of the local player in VR
/// </summary>
public partial class VRArmsConfiguration : Configuration
{
    /// <summary>
    /// Bone of the head
    /// </summary>
    [ModelBone]
    public StringId HeadBone { get; private set; } = "HeadBoneParent";

    /// <summary>
    /// Bones and offsets of the left hand
    /// </summary>
    public Hand LeftHand { get; private set; }

    /// <summary>
    /// Bones and offsets of the right hand
    /// </summary>
    public Hand RightHand { get; private set; }

    /// <summary/>
    [Serialize]
    public partial struct Hand
    {
        /// <summary>
        /// Transform of the wrist bone relative to the tracked (raw) controller pose
        /// </summary>
        public RelativeTransformWithEulerHint WristOffset;

        /// <summary>
        /// Bone that is moved to the tracked controller
        /// </summary>
        [ModelBone]
        public StringId WristBone;

        /// <summary>
        /// Forearm twist bone that follows the wrist
        /// </summary>
        [ModelBone]
        public StringId ElbowPart1Bone;

        /// <summary>
        /// Fingers that are curled by the tracked finger curl. The index, middle and pinky fingers are required to determine the palm direction
        /// </summary>
        public ImmutableArray<FingerBones> Fingers;

        public SkeletonBone GetKnuckle(Skeleton skeleton, Finger finger)
        {
            foreach (var bones in Fingers)
            {
                if (bones.Finger == finger && bones.Joints.Length > 0)
                    return skeleton.GetBone(bones.Joints[0].Bone);
            }

            throw new Exception($"{finger} finger is not configured");
        }
    }

    /// <summary/>
    [Serialize]
    public partial struct FingerBones
    {
        public Finger Finger;

        /// <summary>
        /// Joints of the finger, ordered from the knuckle to the tip
        /// </summary>
        public ImmutableArray<FingerJoint> Joints;

        /// <summary>
        /// Bone at the tip of the finger, used for the direction of the last joint
        /// </summary>
        [ModelBone]
        public StringId TipBone;
    }

    /// <summary/>
    [Serialize]
    public partial struct FingerJoint
    {
        [ModelBone]
        public StringId Bone;

        /// <summary>
        /// Rotation of the joint when the finger is fully curled
        /// </summary>
        [Angle]
        public float CurlAngle;
    }

    /// <summary>
    /// Digits of a hand
    /// </summary>
    public enum Finger
    {
        Thumb = 0,
        Index = 1,
        Middle = 2,
        Ring = 3,
        Pinky = 4
    }
}
