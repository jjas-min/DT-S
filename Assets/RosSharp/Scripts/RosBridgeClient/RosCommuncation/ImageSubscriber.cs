using UnityEngine;
using UnityEngine.UI;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class ImageSubscriber : UnitySubscriber<MessageTypes.Sensor.CompressedImage>
    {
        public RawImage rawImage;

        private Texture2D texture2D;
        private byte[] imageData;
        private bool isMessageReceived;

        protected override void Start()
        {
            base.Start();
            texture2D = new Texture2D(1, 1);
            rawImage.texture = texture2D;
        }

        private void Update()
        {
            if (isMessageReceived)
                ProcessMessage();
        }

        protected override void ReceiveMessage(MessageTypes.Sensor.CompressedImage compressedImage)
        {
            imageData = compressedImage.data;
            isMessageReceived = true;
        }

        private void ProcessMessage()
        {
            texture2D.LoadImage(imageData);
            texture2D.Apply();
            isMessageReceived = false;
        }
    }
}
