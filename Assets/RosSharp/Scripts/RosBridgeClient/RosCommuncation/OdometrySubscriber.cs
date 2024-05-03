using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class OdometrySubscriber : UnitySubscriber<MessageTypes.Nav.Odometry>
    {
        public GameObject PrefabToInstantiate;
        public GameObject Robot;
        private Vector3 position;
        private Quaternion rotation;
        private bool isMessageReceived;

        private GameObject pointsParent;

        protected override void Start()
        {
            base.Start();
            pointsParent = new GameObject("Points");
        }

        private void Update()
        {
            if (isMessageReceived)
                ProcessMessage();
            // else
            //     Debug.Log("no message");
        }

        protected override void ReceiveMessage(MessageTypes.Nav.Odometry message)
        {
            position = GetPosition(message).Ros2Unity();
            rotation = GetRotation(message).Ros2Unity();
            isMessageReceived = true;
        }
        private void ProcessMessage()
        {
            // Debug.Log(position);
            if(!isMessageReceived) return;

            
            // position.z *= -1;
            position.x *= 12.84f;
            position.z *= 12.84f;
            position.x -= 7.12f; //11.78  +  11.03
            position.x += 1.52f;
            position.x += 2.06f;
            position.y -= 3.1f; //11.78  +  11.03
            position.z -= 0.63f;
            position.z -= 0.51f;
            position.z -= 0.37f;
            // position.x *= 1.15f;
            // position.z *= 1.15f;
            if(Mathf.Abs(position.x) > 30 || Mathf.Abs(position.y) > 30 || Mathf.Abs(position.z) > 30) {
                Debug.Log(position);
                Debug.Log("too far");
                return;
            }
            
            // Pink Prefab
            GameObject instantiatedPrefab = Instantiate(PrefabToInstantiate, position, rotation);
            instantiatedPrefab.transform.SetParent(pointsParent.transform);

            // Robot
            Robot.transform.position = position;
            // Robot.transform.Quaternion.w = rotation.w;
            Quaternion originalQuaternion = Robot.transform.rotation;
            Quaternion newQuaternion = new Quaternion(originalQuaternion.x, originalQuaternion.y, originalQuaternion.z, rotation.w);
            Robot.transform.rotation = newQuaternion;
        }

        private Vector3 GetPosition(MessageTypes.Nav.Odometry message)
        {

            return new Vector3(
                (float)message.pose.pose.position.x,
                (float)message.pose.pose.position.y,
                (float)message.pose.pose.position.z);
        }

        private Quaternion GetRotation(MessageTypes.Nav.Odometry message)
        {
            Quaternion q = new Quaternion(
                0,
                1,
                0,
                (float)message.pose.pose.orientation.w);

            return q;
        }
    }
}
