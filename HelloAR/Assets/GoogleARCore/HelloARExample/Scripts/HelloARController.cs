//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.HelloAR
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using GoogleARCore;

    /// <summary>
    /// Controlls the HelloAR example.
    /// </summary>
    public class HelloARController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera.
        /// </summary>
        public Camera m_firstPersonCamera;
        public int mainSpawn = 0;
        public int fencespawn = 0;
        public int secondSpawn = 0;
        private float fencex;
        private float fencez;
        GameObject fence;
        bool readyPlayer1;
        bool readyPlayer2;
        bool boothReady;


        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject m_trackedPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject m_andyAndroidPrefab;
        public GameObject Toclone;
        public GameObject focus;
        //public TextMesh info;
        //public TextMesh xny;
        //public TextMesh fencecounter;
        //public TextMesh infodisplay;
        //public TextMesh treepos;
        //public TextMesh debugger;
        //public TextMesh currentSpawn;


        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject m_searchingForPlaneUI;

        private List<TrackedPlane> m_newPlanes = new List<TrackedPlane>();

        private List<TrackedPlane> m_allPlanes = new List<TrackedPlane>();

        private Color[] m_planeColors = new Color[] {
            new Color(1.0f, 1.0f, 1.0f),
            new Color(0.956f, 0.262f, 0.211f),
            new Color(0.913f, 0.117f, 0.388f),
            new Color(0.611f, 0.152f, 0.654f),
            new Color(0.403f, 0.227f, 0.717f),
            new Color(0.247f, 0.317f, 0.709f),
            new Color(0.129f, 0.588f, 0.952f),
            new Color(0.011f, 0.662f, 0.956f),
            new Color(0f, 0.737f, 0.831f),
            new Color(0f, 0.588f, 0.533f),
            new Color(0.298f, 0.686f, 0.313f),
            new Color(0.545f, 0.764f, 0.290f),
            new Color(0.803f, 0.862f, 0.223f),
            new Color(1.0f, 0.921f, 0.231f),
            new Color(1.0f, 0.756f, 0.027f)
        };
        public void Start()
        {
            mainSpawn = 0;
            fencespawn = 0;
            readyPlayer1 = false;
            readyPlayer2 = false;
            boothReady = false;

        }
        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _QuitOnConnectionErrors();

            // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
            if (Frame.TrackingState != FrameTrackingState.Tracking)
            {
                const int LOST_TRACKING_SLEEP_TIMEOUT = 15;
                Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;
                return;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (mainSpawn < 1)
            {
                Frame.GetNewPlanes(ref m_newPlanes);
            }
            //info.text = mainSpawn.ToString();
            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            for (int i = 0; i < m_newPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                // coordinates.
                if (mainSpawn >= 1)
                {

                    break;
                }
                else
                {
                    mainSpawn += 1;
                    GameObject planeObject = Instantiate(m_trackedPlanePrefab, Vector3.zero, Quaternion.identity,
                        transform);
                    planeObject.GetComponent<TrackedPlaneVisualizer>().SetTrackedPlane(m_newPlanes[i]);

                    // Apply a random color and grid rotation.
                    planeObject.GetComponent<Renderer>().material.SetColor("_GridColor", new Color(0f, 0.588f, 0.533f));
                    planeObject.GetComponent<Renderer>().material.SetFloat("_UvRotation", Random.Range(0.0f, 360.0f));

                }
            }
            // Disable the snackbar UI when no planes are valid.
            bool showSearchingUI = true;
            Frame.GetAllPlanes(ref m_allPlanes);
            for (int i = 0; i < m_allPlanes.Count; i++)
            {
                if (m_allPlanes[i].IsValid)
                {
                    showSearchingUI = false;
                    break;
                }
            }
            if (m_allPlanes.Count > 0 && fencespawn < 1)
            {
                fencespawn++;
                fence = (GameObject)Resources.Load("Fence");
                var plane = m_allPlanes[0];
                Vector3 center = plane.Position;
                float x = plane.Bounds.x;
                float z = plane.Bounds.y;
                fencex = x;
                fencez = z;
                var anchor = Session.CreateAnchor(center, Quaternion.identity);
                if (x <= z)
                {

                    x /= 1.5f;
                    var scale = fence.transform.localScale;
                    scale.x = x;
                    scale.z = x;
                    scale.y = x;
                    fence.transform.localScale = scale;
                }
                else
                {
                    z /= 1.5f;
                    fence.transform.localScale = new Vector3(z, z, z);
                }
                var andyObject = Instantiate(fence, center, Quaternion.identity,
                    anchor.transform);
                andyObject.transform.LookAt(m_firstPersonCamera.transform);
                andyObject.transform.rotation = Quaternion.Euler(0.0f,
                    andyObject.transform.rotation.eulerAngles.y, andyObject.transform.rotation.z);

                // Use a plane attachment component to maintain Andy's y-offset from the plane
                // (occurs after anchor updates).
                andyObject.GetComponent<PlaneAttachment>().Attach(plane);
                fence = andyObject;
            }
            if (fence != null)
            {
                resizeFence();
                groundtrees();
            }
            //m_searchingForPlaneUI.SetActive(showSearchingUI);
            if (Input.touchCount < 1)
            {
                // info.text = "not touched";
                return;
            }
            Touch touch;
            touch = Input.GetTouch(0);
            RaycastHit hits;
            Ray ray = m_firstPersonCamera.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out hits, 500))
            {
                string i = hits.transform.tag;
                //info.text = hits.transform.tag;
                if (i == "Box")
                {
                    //currentSpawn.text = hits.transform.name;

                    Toclone = (GameObject)Resources.Load(hits.transform.name);
                    return;
                }
            }

            TrackableHit hit;
            TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds | TrackableHitFlag.PlaneWithinPolygon;

            if (Session.Raycast(m_firstPersonCamera.ScreenPointToRay(touch.position), raycastFilter, out hit) && Toclone != null)
            {
                if (touch.phase.Equals(TouchPhase.Began))
                {
                    if (GameObject.FindGameObjectsWithTag("Character").Length > 0 && Toclone.transform.tag.Equals("Character"))
                    {
                        return;
                    }
                    if (Physics.Raycast(ray, out hits, 500))
                    {
                        if (hits.transform.tag.Equals("Interactable"))
                        {
                            focus = hits.transform.gameObject;
                            return;
                        }
                    }
                    // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                    // world evolves.
                    var set = hit.Point;
                    var anchor = Session.CreateAnchor(set, Quaternion.identity);

                    // Intanstiate an Andy Android object as a child of the anchor; it's transform will now benefit
                    // from the anchor's tracking.
                    if (Toclone.transform.name.Equals("River") && m_allPlanes.Count > 0)
                    {
                        var plane = m_allPlanes[0];
                        float x = plane.Bounds.x;
                        Vector3 center = plane.Position;
                        set.x = center.x;
                        var scale = Toclone.transform.localScale;
                        scale.x = x;
                        Toclone.transform.localScale = scale;

                    }
                    if (Toclone.transform.name.StartsWith("tre"))
                    {
                        int trenmbr = Random.Range(1, 11);
                        string clonename = "tree00" + trenmbr.ToString();
                        //currentSpawn.text = clonename;
                        Toclone = (GameObject)Resources.Load(clonename);
                        var scale = Toclone.transform.localScale;
                        scale = new Vector3(0.04f, 0.04f, 0.04f);
                        Toclone.transform.localScale = scale;
                    }
                    if (Toclone.transform.name.Equals("Fence") && m_allPlanes.Count > 0)
                    {
                        var plane = m_allPlanes[0];
                        Vector3 center = plane.Position;
                        set.x = center.x;
                        set.z = center.z;
                        float x = plane.Bounds.x;
                        float z = plane.Bounds.y;
                        if (x <= z)
                        {
                            x /= 1.5f;
                            var scale = Toclone.transform.localScale;
                            scale.x = x;
                            scale.z = x;
                            scale.y = x;
                            Toclone.transform.localScale = scale;
                            //currentSpawn.text = x.ToString();
                        }
                        else
                        {
                            z /= 1.5f;
                            Toclone.transform.localScale = new Vector3(z, z, z);
                            //currentSpawn.text = (x/2).ToString();

                        }

                    }
                    /*
                     *------------------------------------------------------
                     * ----------------SPAWNING-----------------------------
                     * -----------------------------------------------------
                     */
                    var andyObject = Instantiate(Toclone, set, Quaternion.identity,
                        fence.transform);
                    SendItemToMulitplayer(Toclone, set.x, set.y, set.z);
                    if (Toclone.transform.tag.Equals("Character"))
                    {
                        mainSpawn++;
                    }

                    // Andy should look at the camera but still be flush with the plane.
                    andyObject.transform.LookAt(m_firstPersonCamera.transform);
                    andyObject.transform.rotation = Quaternion.Euler(0.0f,
                        andyObject.transform.rotation.eulerAngles.y, andyObject.transform.rotation.z);

                    // Use a plane attachment component to maintain Andy's y-offset from the plane
                    // (occurs after anchor updates).
                    andyObject.GetComponent<PlaneAttachment>().Attach(hit.Plane);
                }
                if (touch.phase.Equals(TouchPhase.Moved) && focus != null)
                {
                    focus.transform.position = hit.Point;
                }
                if (touch.phase.Equals(TouchPhase.Ended) && focus != null)
                {
                    focus = null;
                }
            }
        }
        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            // Do not update if ARCore is not tracking.
            if (Session.ConnectionState == SessionConnectionState.DeviceNotSupported)
            {
                _ShowAndroidToastMessage("This device does not support ARCore.");
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed)
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                Application.Quit();
            }
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        /// <param name="length">Toast message time length.</param>
        private static void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
        private void resizeFence()
        {
            if (m_allPlanes.Count < 1 || fence == null)
            {
                return;
            }
            var plane = m_allPlanes[0];
            var tmpx = plane.Bounds.x;
            var tmpz = plane.Bounds.y;
            if (fencex == tmpx && fencez == tmpz)
            {
                return;
            }
            fence.transform.position = plane.Position;
            if (tmpx <= tmpz)
            {
                fencex = tmpx;
                fencez = tmpx;
                tmpx /= 1.5f;
                var scale = fence.transform.lossyScale;
                scale.x = tmpx;
                scale.z = tmpx;
                scale.y = tmpx;
                fence.transform.localScale = scale;

                //currentSpawn.text = x.ToString();
            }
            if (tmpx > tmpz)
            {
                fencex = tmpz;
                fencez = tmpz;
                tmpz /= 1.5f;
                fence.transform.localScale = new Vector3(tmpz, tmpz, tmpz);
                //currentSpawn.text- = (x/2).ToString();

            }

        }
        private void groundtrees()
        {
            GameObject[] trees = GameObject.FindGameObjectsWithTag("Interactable");
            float theY = fence.transform.position.y;
            foreach (GameObject groundme in trees)
            {
                var groundpos = groundme.transform.position;
                groundpos.y = theY;
                groundme.transform.position = groundpos;
            }
        }
        public void CreateItemFromMultiplayer(float what, float x, float y, float z)
        {
            Vector3 relativepos = (fence.transform.position + new Vector3(x, y, z));
            if (m_allPlanes.Count < 1)
            {
                return;
            }
            int number = (int)what;
            //establish type of object
            GameObject theClone = null;
            var anchor = Session.CreateAnchor(relativepos, Quaternion.identity);
            if (number < 11)
            {
                theClone = (GameObject)Resources.Load("tree00" + number);
            }
            if (number == 11)
            {
                theClone = (GameObject)Resources.Load("Candy");

            }
            if (number == 12)
            {
                theClone = (GameObject)Resources.Load("Knyttet");
            }
            if (number == 13)
            {
                //theClone = (GameObject)Resources.Load("Cloud");
                //delete below return if we have a cloud
                return;
            }
            var cloneObject = Instantiate(theClone, fence.transform.position, Quaternion.identity, fence.transform);
            cloneObject.transform.position = (fence.transform.position + new Vector3(x, 0, z)) * fence.transform.localScale.x;
            var pos = cloneObject.transform.position;
            pos.y /= fence.transform.localScale.x;
            cloneObject.transform.position = pos;
            cloneObject.transform.LookAt(m_firstPersonCamera.transform);
            cloneObject.transform.rotation = Quaternion.Euler(0.0f, cloneObject.transform.rotation.eulerAngles.y, cloneObject.transform.rotation.z);
            var scale = new Vector3(0.04f, 0.04f, 0.04f);
            if (cloneObject.name.StartsWith("tre"))
            {
                cloneObject.transform.localScale = scale;
            }
            cloneObject.GetComponent<PlaneAttachment>().Attach(m_allPlanes[0]);

        }
        private void SendItemToMulitplayer(GameObject what, float x, float y, float z)
        {
            Vector3 relativepos = fence.transform.position;
            relativepos.x = (x - relativepos.x) / fence.transform.localScale.x;
            relativepos.y = (y - relativepos.y) / fence.transform.localScale.x;
            relativepos.z = (z - relativepos.z) / fence.transform.localScale.x;
            int number = 0;
            if (what.name.StartsWith("tre"))
            {

                string i = what.name.Substring(what.name.Length - 1, 1);
                number = int.Parse(i);
                if (number == 0)
                {
                    number = 10;
                }


                float[] list = { number, relativepos.x, relativepos.y, relativepos.z };
                GameObject.Find("telephone").GetComponent<SendPositionOnUpdate>().SendInformation(list);
                //send message
            }
            //if i don't work delete me
            if (what.name.StartsWith("Cand"))
            {
                number = 11;
                float[] list = { number, relativepos.x, relativepos.y, relativepos.z };
                GameObject.Find("telephone").GetComponent<SendPositionOnUpdate>().SendInformation(list);
            }
            if (what.name.StartsWith("Knyt"))
            {
                number = 12;
                float[] list = { number, relativepos.x, relativepos.y, relativepos.z };
                GameObject.Find("telephone").GetComponent<SendPositionOnUpdate>().SendInformation(list);
            }
            if (what.name.StartsWith("Clou"))
            {
                number = 13;
                float[] list = { number, relativepos.x, relativepos.y, relativepos.z };
                GameObject.Find("telephone").GetComponent<SendPositionOnUpdate>().SendInformation(list);
            }

        }
    }
}
