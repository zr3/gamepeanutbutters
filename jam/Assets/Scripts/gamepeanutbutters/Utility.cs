using UnityEngine;

namespace Assets.Scripts
{
    public static class Utility
    {
        public static void PhysicalDestroy(GameObject gob)
        {
            gob.transform.position = new Vector3(0, -1000, 0);
            GameObject.Destroy(gob, 0.1f);
        }

        public static void SendMessageToChildren(Transform gob, string message, SendMessageOptions options)
        {
            gob.SendMessage(message, options);
            for (int i = 0; i < gob.childCount; ++i)
            {
                gob.GetChild(i).SendMessage(message, options);
            }
        }

        public static void SendMessageToChildren(Transform gob, string message, object param, SendMessageOptions options)
        {
            gob.SendMessage(message, param, options);
            for (int i = 0; i < gob.childCount; ++i)
            {
                gob.GetChild(i).SendMessage(message, param, options);
            }
        }
    }
}
