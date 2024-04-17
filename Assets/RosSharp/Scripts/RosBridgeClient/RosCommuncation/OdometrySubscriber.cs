using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class OdometrySubscriber : UnitySubscriber<MessageTypes.Nav.Odometry>
    {
        public Transform RobotTransform;

        private Vector3 position;
        private Quaternion rotation;
        private bool isMessageReceived;

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            if (isMessageReceived)
                ProcessMessage();
        }

        protected override void ReceiveMessage(MessageTypes.Nav.Odometry message)
        {
            position = GetPosition(message).Ros2Unity();
            rotation = GetRotation(message).Ros2Unity();
            isMessageReceived = true;
        }

        private void ProcessMessage()
        {
            // 포지션 편집
            position.z *= -1;
            position.x *= 7.75f;
            position.z *= 7.75f;
            position.x += 11.78f;
            position.z -= 16.74f;

            // 로봇의 위치 및 회전 적용
            RobotTransform.position = position;
            RobotTransform.rotation = rotation;
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
            // 메시지에서 받은 회전값
            Quaternion messageRotation = new Quaternion(
                0,
                1,
                0,
                (float)message.pose.pose.orientation.w
            );

            return messageRotation;
        }
    }
}
