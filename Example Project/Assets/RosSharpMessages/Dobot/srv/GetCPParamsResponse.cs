/* 
 * This message is auto generated by ROS#. Please DO NOT modify.
 * Note:
 * - Comments from the original code will be written in their own line 
 * - Variable sized arrays will be initialized to array of size 0 
 * Please report any issues at 
 * <https://github.com/siemens/ros-sharp> 
 */

using Newtonsoft.Json;

namespace RosSharp.RosBridgeClient.MessageTypes.Dobot
{
    public class GetCPParamsResponse : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "dobot_msgs/GetCPParams";

        public int result;
        public float planAcc;
        public float junctionVel;
        public float acc;
        public byte realTimeTrack;

        public GetCPParamsResponse()
        {
            this.result = 0;
            this.planAcc = 0.0f;
            this.junctionVel = 0.0f;
            this.acc = 0.0f;
            this.realTimeTrack = 0;
        }

        public GetCPParamsResponse(int result, float planAcc, float junctionVel, float acc, byte realTimeTrack)
        {
            this.result = result;
            this.planAcc = planAcc;
            this.junctionVel = junctionVel;
            this.acc = acc;
            this.realTimeTrack = realTimeTrack;
        }
    }
}